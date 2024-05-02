using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace NNArbetsProv
{
    //beacuse of the NULL in the calc arc and CSV is not nice, ValidUntil is a string
    //Don't have more time
    public class PriceDetails
    {
        [Index(0)]
        public int PriceValueId         { get; set; }
        [Index(1)]
        public DateTime Created         { get; set; }
        [Index(2)]
        public DateTime Modified        { get; set; }
        [Index(3)]
        public string CatalogEntryCode  { get; set; }
        [Index(4)]
        public string MarketId          { get; set; }
        [Index(5)]
        public string CurrencyCode      { get; set; }
        [Index(6)]
        public DateTime ValidFrom       { get; set; }
        [Index(7)]
        public string ValidUntil        { get; set; }
        [Index(8)]
        public decimal UnitPrice        { get; set; }
    }

    public class PriceDetailOutput
    {
        
        public string MarketId      { get; set; }
        public decimal UnitPrice    { get; set; }
        public string CurrencyCode  { get; set; }
        public Nullable<DateTime> Start       { get; set; }
        public Nullable<DateTime> End         { get; set; }

        //public PriceDetailOutput(PriceDetails OriginalPriceDetail, Nullable<DateTime> start, Nullable<DateTime> end)
        //{
        //    this.MarketId = OriginalPriceDetail.MarketId;
        //    this.UnitPrice = OriginalPriceDetail.UnitPrice;
        //    this.CurrencyCode = OriginalPriceDetail.CurrencyCode;
        //    this.Start = start;
        //    this.End = end;
        //}
        public PriceDetailOutput(PriceDetails OriginalPriceDetail, Nullable<DateTime> start)
        {
            this.MarketId = OriginalPriceDetail.MarketId;
            this.UnitPrice = OriginalPriceDetail.UnitPrice;
            this.CurrencyCode = OriginalPriceDetail.CurrencyCode;
            this.Start = start;
            this.End = null;
        }
        public PriceDetailOutput(PriceDetails OriginalPriceDetail)
        {
            this.MarketId = OriginalPriceDetail.MarketId;
            this.UnitPrice = OriginalPriceDetail.UnitPrice;
            this.CurrencyCode = OriginalPriceDetail.CurrencyCode;
            this.Start = null;
            this.End = null;
        }
    }

    /// <summary>
    /// Class that helps convert/map getRecords<> into PriceDetails
    /// </summary>
    public class PriceDetailsMap : ClassMap<PriceDetails>
    {
        public PriceDetailsMap()
        {
            Map(m => m.PriceValueId).Index(0);
            Map(m => m.Created).Index(1).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss.fffffff");
            Map(m => m.Modified).Index(2).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss.fffffff");
            Map(m => m.CatalogEntryCode).Index(3);
            Map(m => m.MarketId).Index(4);
            Map(m => m.CurrencyCode).Index(5);
            Map(m => m.ValidFrom).Index(6).TypeConverterOption.Format("yyyy-MM-dd HH:mm:ss.fffffff");
            Map(m => m.ValidUntil).Index(7);
            Map(m => m.UnitPrice).Index(8);
        }
    }
}
