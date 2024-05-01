using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace NNArbetsProv.Pages
{
    public class SellingPrice
    {
        private ILogger<IndexModel> _logger;
        public void giveLogger(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public string test()
        {
            return "Nordic Nest Rock";
        }
        public void readInExcel(string fileName)
        {
            CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
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

            csv.Read();

            var records = csv.GetRecord<dynamic>();

            foreach (var record in records)
            {
                Console.WriteLine(record);
            }

        }
        public void getWithSKU(string SKU)
        {

        }
        public void getWithSKU(string SKU, string fileName)
        {

        }
    }
}
