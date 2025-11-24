using Microsoft.AspNetCore.Mvc;
using savemoney.Models;
using System.Collections.Generic;
using savemoney.Services;

namespace savemoney.Controllers;

public class NoticiasController : Controller
{
    private readonly NoticiasService _noticiasService;

    public NoticiasController(NoticiasService noticiasService)
    {
        _noticiasService = noticiasService;
    }

    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]

    public async Task<IActionResult> GetNoticias([FromQuery] string query, [FromQuery] int page = 1)
    {

        try
        {

            var noticiasJson = await _noticiasService.BuscarNoticiasAsync(query, page);
            return Content(noticiasJson, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocorreu um erro: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetSugestoes([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {

            return Json(new List<string>());
        }

        try
        {

            var sugestoes = await _noticiasService.BuscarSugestoesAsync(query);
            return Json(sugestoes);
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Erro ao buscar sugestões: {ex.Message}");
            return Json(new List<string>());
        }
    }
}