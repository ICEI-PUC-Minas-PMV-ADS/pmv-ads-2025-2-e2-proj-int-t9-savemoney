using Microsoft.AspNetCore.Mvc;

namespace savemoney.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
