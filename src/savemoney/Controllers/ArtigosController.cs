using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using savemoney.Services;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class ArtigosController : Controller
    {
        private readonly ArtigosService _artigosService;

        public ArtigosController(ArtigosService artigosService)
        {
            _artigosService = artigosService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetArtigos([FromQuery] ArtigoBuscaRequest request)
        {
            try
            {

                var artigosJson = await _artigosService.BuscarArtigosAsync(request);

                return Content(artigosJson, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro interno no servidor: {ex.Message}");
            }
        }
    }
}