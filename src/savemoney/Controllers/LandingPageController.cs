using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class LandingPageController : Controller
    {
        private readonly ILogger<LandingPageController> _logger;
        public LandingPageController(ILogger<LandingPageController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}