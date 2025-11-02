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

    /* * PASSO 1: CORRIGIR ESTE MÉTODO
     * * Rota: /Noticias/GetNoticias
     * Aceita 'query' e 'page' do JavaScript.
    */
    [HttpGet]
    // MUDANÇA 1: A assinatura agora aceita 'query' e 'page'.
    // 'page = 1' define um valor padrão se o front-end não enviar.
    public async Task<IActionResult> GetNoticias([FromQuery] string query, [FromQuery] int page = 1)
    {
        // MUDANÇA 2: Usamos o 'query' recebido. 
        // Se estiver vazio, o service decidirá o padrão (ex: "finanças").
        try
        {
            // MUDANÇA 3: Passamos AMBOS os parâmetros para o service.
            // (Você precisará atualizar o 'BuscarNoticiasAsync' no seu service para aceitar isso)
            var noticiasJson = await _noticiasService.BuscarNoticiasAsync(query, page); 

            return Content(noticiasJson, "application/json");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ocorreu um erro: {ex.Message}");
        }
    }

    /* * PASSO 2: ADICIONAR ESTE NOVO MÉTODO
     * * Rota: /Noticias/GetSugestoes
     * Aceita 'query' do JavaScript e retorna uma lista de strings.
    */
    [HttpGet]
    public async Task<IActionResult> GetSugestoes([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            // Otimização: não buscar sugestões para queries curtas
            return Json(new List<string>()); 
        }

        try
        {
            // (Você precisará criar 'BuscarSugestoesAsync' no seu service)
            var sugestoes = await _noticiasService.BuscarSugestoesAsync(query);
            
            // Retorna um JSON array: ["sugestao1", "sugestao2"]
            return Json(sugestoes); 
        }
        catch (Exception ex)
        {
            // Não quebre a aplicação se as sugestões falharem
            Console.WriteLine($"Erro ao buscar sugestões: {ex.Message}");
            return Json(new List<string>()); // Retorna lista vazia em caso de erro
        }
    }
}