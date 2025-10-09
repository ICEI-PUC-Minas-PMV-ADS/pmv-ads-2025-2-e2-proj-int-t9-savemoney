using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace savemoney.Controllers;

public class FerramentasController : Controller
{
    // GET: /CalculadoraDeMetas/
    public IActionResult CalculadoraDeMetas()
    {
        return View();
    }
    // GET: /CalculadoraDeEquilibrio/
    public IActionResult CalculadoraDeEquilibrio()
    {
        return View();
    }
}