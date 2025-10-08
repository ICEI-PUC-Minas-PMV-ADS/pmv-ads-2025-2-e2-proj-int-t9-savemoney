
using Microsoft.AspNetCore.Mvc;

public class FerramentasController : Controller
{
    // Retorna a página da calculadora de metas
    public IActionResult CalculadoraMetas()
    {
        return View(); // Mostra o arquivo CalculadoraMetas.cshtml
    }

    // retorna a página da calculadora de ponto de equilibrio
    public IActionResult CalculadoraDeEquilibrio()
    {
        return View(); // mostra o arquivo PontoDeEquilibro.cshtml
    }
}