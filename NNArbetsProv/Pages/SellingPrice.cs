using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace NNArbetsProv.Pages
{
    public class SellingPrice
    {
        private ILogger<IndexModel> _logger;

        //Product(PriceDetails) are in a Dictionary where the Key is the SKU and the value is a list of the PriceDetails on that product
        //Could do more in depth aka
        //private Dictionary<string, Dictionary<List<PriceDetails>>> Product;
        //But think it's to overkill and would take more memory than needed
        private Dictionary<string, List<PriceDetails>> Product;

        private void OrderListByValidFrom(ref List<PriceDetails> thePriceDetailListCpy)
        {
            thePriceDetailListCpy.Sort((x, y) => x.ValidFrom.CompareTo(y.ValidFrom));
        }

        private Nullable<DateTime> FromStringToDateTime(string dateTimeStr)
        {
            return dateTimeStr == "NULL" ? null : DateTime.Parse(dateTimeStr);
        }
        
        private bool lessThanTime(Nullable<DateTime> d1, Nullable<DateTime> d2)
        {
            // It should NOT be like this but we hack it, btw no Idea if null is > *anytime at all (Not enough time to check)
            if(d1 == null)
                return false;
            if (d2 == null)
                return true;

            return d1 < d2;
        }

        public void Init(ILogger<IndexModel> logger)
        {
            Product = new Dictionary<string, List<PriceDetails>>();
            _logger = logger;
        }

        //Read the CSV file and with an output with the SearchOptions to easier search for everything
        //incase of more market ID:s, Currency codes a.s.o are added
        public SearchOptions readInCSV(string fileName)
        {
            SearchOptions theReturn = new SearchOptions();

            //CSVhelper library init///////////
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "\t",
                HasHeaderRecord = true
            };

            StreamReader reader;
            CsvReader csv;
            try
            {
                reader = new StreamReader(fileName);
                csv = new CsvReader(reader, csvConfig);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return theReturn;
            }

            csv.Context.RegisterClassMap<PriceDetailsMap>();

            csv.Read();
            csv.ReadHeader();

            var header = csv.Parser.RawRecord;

            var records = csv.GetRecords<PriceDetails>();

            //////////////////////////////////////

            //Add each PriceDetail in the Dictionary
            //Very ugly but don't know another way
            foreach (var record in records)
            {
                if(!Product.ContainsKey(record.CatalogEntryCode))
                {
                    Product.Add(record.CatalogEntryCode, new List<PriceDetails>());
                    theReturn.SKU.Add(record.CatalogEntryCode);
                }
                theReturn.marketId.Add(record.MarketId);
                theReturn.currency.Add(record.CurrencyCode);
                Product[record.CatalogEntryCode].Add(record);
            }

            return theReturn;
        }

        //Get the Object with help of SKU, MarketId and CurrencyCode
        public List<PriceDetailOutput> getObject(string SKU, string MarketId, string CurrencyCode)
        {
            List<PriceDetails> priceDetailListref = null;
            
            //Check if SKU/Product exist
            if(SKU == null || !Product.TryGetValue(SKU, out priceDetailListref))
            {
                //return empty if it didn't exist
                return new List<PriceDetailOutput>();
            }

            List<PriceDetails> priceDetailListCpy = new List<PriceDetails>(priceDetailListref);
            List<PriceDetailOutput> Table = new List<PriceDetailOutput>();

            //Remove unwanted in copies, but doesn't do it by reference!
            //Don't know if this takes to much power...
            for(int i = 0; i < priceDetailListCpy.Count; i++)
            {
                if (priceDetailListCpy[i].MarketId != MarketId || priceDetailListCpy[i].CurrencyCode != CurrencyCode)
                {
                    priceDetailListCpy.RemoveAt(i);
                    i--;
                }
            }

            if(priceDetailListCpy.Count < 1)
            {
                return new List<PriceDetailOutput>();
            }

            //Make them in order by "validFrom"
            OrderListByValidFrom(ref priceDetailListCpy);

            //Make DateTime Nullable because validUntil can be NULL, and made them both so it looks nicer
            int currentIndexPrice = 0;
            int nextIndexPrice = 1;
            Nullable<DateTime> currentTime = priceDetailListCpy[currentIndexPrice].ValidFrom;
            Nullable<DateTime> validUntil = FromStringToDateTime(priceDetailListCpy[currentIndexPrice].ValidUntil);

            //Add the first one
            Table.Add(new PriceDetailOutput(priceDetailListCpy[currentIndexPrice], currentTime));

            while (nextIndexPrice < priceDetailListCpy.Count)
            {
                //check if nextIndexPrice.validfrom is > validuntil
                //if true jump back some and add it to the table
                if(priceDetailListCpy[nextIndexPrice].ValidFrom > validUntil)
                {
                    currentTime = validUntil;
                    //EW but hack
                    while (priceDetailListCpy[nextIndexPrice].ValidFrom > validUntil)
                    {
                        currentIndexPrice--;
                        validUntil = FromStringToDateTime(priceDetailListCpy[currentIndexPrice].ValidUntil);
                    }
                    
                    Table.Last().End = currentTime;
                    Table.Add(new PriceDetailOutput(priceDetailListCpy[currentIndexPrice], currentTime));
                }

                //Check if nextIndexPrice.price is < currentIndexPrice.price
                //if true add it
                if(priceDetailListCpy[nextIndexPrice].UnitPrice < Table.Last().UnitPrice)
                {

                    //Check EdgeCase, if Validfrom exist before 
                    //also bigger check if this is done more than twice (it doesn't happen though in this test)
                    {
                        decimal tempCurrentPrice = priceDetailListCpy[nextIndexPrice].UnitPrice;
                        int nextIndexPriceTemp = nextIndexPrice;

                        while (nextIndexPriceTemp < priceDetailListCpy.Count - 1 &&
                            priceDetailListCpy[nextIndexPriceTemp].ValidFrom == priceDetailListCpy[(nextIndexPriceTemp + 1)].ValidFrom)
                        {
                            if (tempCurrentPrice > priceDetailListCpy[(nextIndexPriceTemp + 1)].UnitPrice)
                            {
                                tempCurrentPrice = priceDetailListCpy[(nextIndexPriceTemp + 1)].UnitPrice;
                                nextIndexPrice = nextIndexPriceTemp;
                            }
                            nextIndexPriceTemp++;
                        }
                    }

                    currentTime = priceDetailListCpy[nextIndexPrice].ValidFrom;
                    Table.Last().End = currentTime;

                    validUntil = FromStringToDateTime(priceDetailListCpy[nextIndexPrice].ValidUntil);
                    Table.Add(new PriceDetailOutput(priceDetailListCpy[nextIndexPrice], currentTime));
                    currentIndexPrice = nextIndexPrice;
                }


                nextIndexPrice++;
            }

            //Do the last small things like add End time to table.last
            //and add the price detail with the longest ValidUntil
            Table.Last().End = FromStringToDateTime(priceDetailListCpy[currentIndexPrice].ValidUntil);

            Nullable<DateTime> biggest = Table.Last().End;

            for(int i = priceDetailListCpy.Count - 1; i >= 0; i--)
            {
                if (lessThanTime(biggest, FromStringToDateTime(priceDetailListCpy[i].ValidUntil)))
                {
                    currentIndexPrice = i;
                    biggest = FromStringToDateTime(priceDetailListCpy[i].ValidUntil);
                }
            }
            if(biggest != Table.Last().End) 
            {
                Table.Add(new PriceDetailOutput(priceDetailListCpy[currentIndexPrice], Table.Last().End));
                Table.Last().End = biggest;
            }

            return Table;
        }
    }
}
