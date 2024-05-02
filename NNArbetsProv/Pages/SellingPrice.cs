using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NNArbetsProv.Pages
{
    public class SellingPrice
    {
        private ILogger<IndexModel> _logger;
        private Dictionary<string, List<PriceDetails>> Product;
        //private Dictionary<string, Dictionary<string, List<PriceDetails>>> Product;

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
            // It should be like this but we hack it
            if(d1 == null)
                return false;
            if (d2 == null)
                return true;

            return d1 < d2;
        }

        public void giveLogger(ILogger<IndexModel> logger)
        {
            Product = new Dictionary<string, List<PriceDetails>>();
            _logger = logger;
        }
        public void readInExcel(string fileName)
        {
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
                return;
            }

            csv.Context.RegisterClassMap<PriceDetailsMap>();

            csv.Read();
            csv.ReadHeader();

            var header = csv.Parser.RawRecord;

            var records = csv.GetRecords<PriceDetails>();

            foreach (var record in records)
            {
                if(!Product.ContainsKey(record.CatalogEntryCode))
                {
                    Product.Add(record.CatalogEntryCode, new List<PriceDetails>());
                }
                Product[record.CatalogEntryCode].Add(record);
            }
            _logger.LogInformation("Done");
        }
        public List<PriceDetailOutput> getObject(string SKU, string MarketId, string CurrencyCode)
        {
            List<PriceDetails> priceDetailListref = null;
            if(!Product.TryGetValue(SKU, out priceDetailListref))
            {
                //return empty if it didn't exist
                return new List<PriceDetailOutput>();
            }
            List<PriceDetails> priceDetailListCpy = new List<PriceDetails>(priceDetailListref);

            List<PriceDetailOutput> Table = new List<PriceDetailOutput>();
            //Do logic here

            //Remove unwanted in copies, but doesn't do it by reference
            for(int i = 0; i < priceDetailListCpy.Count; i++)
            {
                if (priceDetailListCpy[i].MarketId != MarketId || priceDetailListCpy[i].CurrencyCode != CurrencyCode)
                {
                    priceDetailListCpy.RemoveAt(i);
                    i--;
                }
            }

            //Make them in order by validFrom
            OrderListByValidFrom(ref priceDetailListCpy);

            int currentIndexPrice = 0;
            int nextIndexPrice = 1;
            Nullable<DateTime> currentTime = priceDetailListCpy[currentIndexPrice].ValidFrom;
            Nullable<DateTime> validUntil = FromStringToDateTime(priceDetailListCpy[currentIndexPrice].ValidUntil);

            //Add the first one
            Table.Add(new PriceDetailOutput(priceDetailListCpy[currentIndexPrice], priceDetailListCpy[currentIndexPrice].ValidFrom));

            //TODO : FIX THIS!!! NOT RIGHT I THINK
            while (priceDetailListCpy.Count > 1)
            {
                 //check if nextIndexPrice.validfrom is > validuntil
                    //if true go back and add
                    //continue
                //else : 

                //Check if nextIndexPrice.price is < currentIndexPrice
                    //if true add it



                nextIndexPrice++;
            }

            return Table;
        }
        public void getWithSKU(string SKU, string fileName)
        {

        }
    }
}
