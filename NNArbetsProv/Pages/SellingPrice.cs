using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace NNArbetsProv.Pages
{
    public class SellingPrice
    {
        private ILogger<IndexModel> _logger;
        private Dictionary<string, List<PriceDetails>> Product;
        //private Dictionary<string, Dictionary<string, List<PriceDetails>>> Product;

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
        public List<List<PriceDetails>> getObject(string SKU, string MarketId, string CurrencyCode)
        {
            List<PriceDetails> priceDetailListref = null;
            if(!Product.TryGetValue(SKU, out priceDetailListref))
            {
                //return empty if it didn't exist
                return new List<List<PriceDetails>>();
            }
            List<PriceDetails> priceDetailListCpy = new List<PriceDetails>(priceDetailListref);

            List<List<PriceDetails>> Table = new List<List<PriceDetails>>();
            //Do logic here

            //Remove unwanted but don't do it by reference!!!!
            //TODO : what's above
            for(int i = 0; i < priceDetailListCpy.Count; i++)
            {
                if (priceDetailListCpy[i].MarketId != MarketId || priceDetailListCpy[i].CurrencyCode != CurrencyCode)
                {
                    priceDetailListCpy.RemoveAt(i);
                    i--;
                }
            }

            return Table;
        }
        public void getWithSKU(string SKU, string fileName)
        {

        }
    }
}
