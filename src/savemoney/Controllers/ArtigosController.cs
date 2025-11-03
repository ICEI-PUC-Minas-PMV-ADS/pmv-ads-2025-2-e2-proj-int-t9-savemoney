using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using savemoney.Services;
using savemoney.Models; // <-- MUDANÇA 1: Adicionada a referência ao DTO

namespace savemoney.Controllers
{
    public class ArtigosController : Controller
    {
        private readonly ArtigosService _artigosService;

        // Injeção de dependência
        public ArtigosController(ArtigosService artigosService)
        {
            _artigosService = artigosService;
        }

        // Action que carrega a página HTML (/Artigos)
        public IActionResult Index()
        {
            return View();
        }

        /* Endpoint da API que o JavaScript vai consumir
           A Rota será /Artigos/GetArtigos
        */
        [HttpGet]
        /* * MUDANÇA 2 (A MUDANÇA PRINCIPAL):
         * Trocamos '([FromQuery] string termo)' por '([FromQuery] ArtigoBuscaRequest request)'.
         * O ASP.NET Core vai ler todos os parâmetros da URL (searchTerm, region, page, etc.)
         * e preencher o objeto 'request' automaticamente.
         */
        public async Task<IActionResult> GetArtigos([FromQuery] ArtigoBuscaRequest request)
        {
            try
            {
                /*
                 * MUDANÇA 3:
                 * Agora passamos o objeto 'request' completo para o serviço.
                 * O serviço (no próximo passo) vai usá-lo para construir a URL da API.
                 */
                var artigosJson = await _artigosService.BuscarArtigosAsync(request);

                // Retorna o JSON
                return Content(artigosJson, "application/json");
            }
            catch (Exception ex)
            {
                // Em caso de erro inesperado no nosso servidor
                return StatusCode(500, $"Ocorreu um erro interno no servidor: {ex.Message}");
            }
        }
    }
}