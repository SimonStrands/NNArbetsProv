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
            _sellingPrice.giveLogger(_logger);
            searchOptions = _sellingPrice.readInExcel("price_detail.csv");
        }

        public string WelcomeMessage { get; private set; }
        public string[] Items { get; private set; }

        public void OnGet()
        {
            _logger.LogInformation("hi");
            Console.WriteLine("hi");
            //_sellingPrice.readInExcel("price_detail.csv");
            //_sellingPrice.getObject("27773-02", "sv", "SEK");
            
        }

        public IActionResult OnPost(string action, 
            string SKUDropDown, string marketIdDropDown, string currencyDropDown,
            string SKUText, string marketIdText, string currencyText)
        {
            if (action == "doSomething")
            {
                string SKU = SKUText != null ? SKUText : SKUDropDown;
                string marketId = marketIdText != null ? marketIdText : marketIdDropDown;
                string currency = currencyText != null ? currencyText : currencyDropDown;
                tableOutput = _sellingPrice.getObject(SKU, marketId, currency);
            }
            return Page();
        }

    }
}

//@foreach(var option in Model.Options)
//{
//    <option value="@option">@option</option>
//}
