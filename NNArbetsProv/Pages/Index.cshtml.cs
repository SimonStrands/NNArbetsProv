using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace NNArbetsProv.Pages
{
    public class SearchOptions
    {
        public HashSet<string> SKU;
        public HashSet<string> marketId;
        public HashSet<string> currency;

        public SearchOptions()
        {
            SKU = new HashSet<string>();
            marketId = new HashSet<string>(); 
            currency = new HashSet<string>(); 
        }
    }
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private SellingPrice _sellingPrice;

        public SearchOptions searchOptions;
        public List<PriceDetailOutput> tableOutput;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            _sellingPrice = new SellingPrice();
            _sellingPrice.Init(_logger);
            searchOptions = _sellingPrice.readInCSV("price_detail.csv");
        }

        public string WelcomeMessage { get; private set; }
        public string[] Items { get; private set; }

        public void OnGet()
        {            
        }

        public IActionResult OnPost(string action, 
            string SKUDropDown, string marketIdDropDown, string currencyDropDown,
            string SKUText, string marketIdText, string currencyText)
        {
            if (action == "GetProductPriceHistory")
            {
                string SKU = SKUText != null ? SKUText : SKUDropDown;
                string marketId = marketIdText != null ? marketIdText : marketIdDropDown;
                string currency = currencyText != null ? currencyText : currencyDropDown;
                tableOutput = _sellingPrice.getObject(SKU, marketId, currency);
            }
            else if(action == "Test")
            {
                foreach(string s in searchOptions.SKU)
                {
                    foreach (string m in searchOptions.marketId)
                    {
                        foreach (string c in searchOptions.currency)
                        {
                            tableOutput = _sellingPrice.getObject(s, m, c);
                        }
                    }
                }
            }
            return Page();
        }

    }
}

//@foreach(var option in Model.Options)
//{
//    <option value="@option">@option</option>
//}
