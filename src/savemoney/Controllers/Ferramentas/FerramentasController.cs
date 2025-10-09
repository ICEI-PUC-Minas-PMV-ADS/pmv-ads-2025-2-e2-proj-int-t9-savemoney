using Microsoft.AspNetCore.Mvc;

namespace ferramentas.Controllers;

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