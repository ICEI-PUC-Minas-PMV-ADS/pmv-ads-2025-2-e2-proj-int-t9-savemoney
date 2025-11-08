using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}