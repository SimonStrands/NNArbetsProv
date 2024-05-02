using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace NNArbetsProv.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private SellingPrice _sellingPrice;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            _sellingPrice = new SellingPrice();
            _sellingPrice.giveLogger(_logger);
        }

        public string WelcomeMessage { get; private set; }
        public string[] Items { get; private set; }

        public void OnGet()
        {

            _sellingPrice.readInExcel("price_detail.csv");
            _sellingPrice.getObject("27773-02", "sv", "SEK");
            
        }

        public IActionResult OnPost(string action)
        {
            if (action == "doSomething")
            {
                DoSomething();
            }
            return Page();
        }

        private void DoSomething()
        {
        
            
            //_logger.LogInformation(_sellingPrice.test());
        }
    }
}
