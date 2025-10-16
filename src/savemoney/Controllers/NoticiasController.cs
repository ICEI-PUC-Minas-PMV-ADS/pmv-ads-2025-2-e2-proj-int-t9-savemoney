using Microsoft.AspNetCore.Mvc;
using savemoney.Models; // Classe DTO
using System.Collections.Generic; // Necessario para utilizar 'List<T>'
using savemoney.Services;

namespace savemoney.Controllers;

public class NoticiasController : Controller
{
    // Controller "Orquesta", recebe o pedido, chama o Service para fazer o trabalho, e devolve a resposta.
    private readonly NoticiasService _noticiasService;

    //o serviço é injetado aqui pelo sistemas e guardado na nossa variável privada
    public NoticiasController(NoticiasService noticiasService)
    {
        _noticiasService = noticiasService;
    }

    // Esse método esta puxando a página
    public IActionResult Index()
    {
        return View();
    }

    /* Este metodo serve para os DADOS JSON para a API
    A Rota será /EducacaoFinanceira/GetNoticias
    */
    [HttpGet]
    public async Task<IActionResult> GetNoticias([FromQuery] string termo)
    {
        var termoDeBusca = string.IsNullOrWhiteSpace(termo) ? "finanças" : termo;

        try
        {
            // Aqui chama o serviço que faz todo o trabalho pesado
            var noticiasJson = await _noticiasService.BuscarNoticiasAsync(termoDeBusca);

            // Aqui ele repassa a resposta.
            return Content(noticiasJson, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocorreu um erro: {ex.Message}");
        }
    }

}