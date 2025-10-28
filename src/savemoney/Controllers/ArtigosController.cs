using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using savemoney.Services;

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
        public async Task<IActionResult> GetArtigos([FromQuery] string termo)
        {
            try
            {
                // Chama o serviço. O serviço já lida com o termo padrão se este for nulo.
                var artigosJson = await _artigosService.BuscarArtigosAsync(termo);

                // Retorna o JSON
                return Content(artigosJson, "application/json");
            }
            catch (Exception ex)
            {
                // Em caso de erro inesperado no nosso servidor (o serviço já trata erros da API externa)
                return StatusCode(500, $"Ocorreu um erro interno no servidor: {ex.Message}");
            }
        }
    }
}