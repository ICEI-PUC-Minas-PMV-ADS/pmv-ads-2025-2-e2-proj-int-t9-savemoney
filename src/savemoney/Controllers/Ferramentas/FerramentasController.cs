using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class FerramentasController : Controller
    {
        private readonly ILogger<FerramentasController> _logger;

        public FerramentasController(ILogger<FerramentasController> logger)
        {
            _logger = logger;
        }

        // GET: /Ferramentas/CalculadoraDeMetas/
        public IActionResult CalculadoraDeMetas()
        {
            return View();
        }

        // GET: /Ferramentas/CalculadoraDeEquilibrio/
        public IActionResult CalculadoraDeEquilibrio()
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