using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace NNArbetsProv.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string WelcomeMessage { get; private set; }
        public string[] Items { get; private set; }

        public void OnGet()
        {
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
            SellingPrice t;
            t = new SellingPrice(); 
            
            _logger.LogInformation(t.test());
        }
    }
}
