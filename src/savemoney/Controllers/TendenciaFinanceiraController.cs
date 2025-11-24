using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using savemoney.Services.Interfaces;

namespace savemoney.Controllers
{
    /// <summary>
    /// Controller responsável pela análise de tendências financeiras
    /// 
    /// SOLID - SRP: Apenas gerencia requisições HTTP, toda lógica está no Service
    /// SOLID - DIP: Depende da interface ITendenciaFinanceiraService, não da implementação
    /// </summary>
    [Authorize] // Requer autenticação
    public class TendenciaFinanceiraController : Controller
    {
        private readonly ITendenciaFinanceiraService _tendenciaService;

        /// <summary>
        /// Construtor com injeção de dependência
        /// </summary>
        public TendenciaFinanceiraController(ITendenciaFinanceiraService tendenciaService)
        {
            _tendenciaService = tendenciaService;
        }

        /// <summary>
        /// GET: TendenciaFinanceira/Index
        /// Exibe a tela inicial de análise de tendências
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            // Apenas renderiza a view inicial (sem dados)
            return View();
        }

        /// <summary>
        /// POST: TendenciaFinanceira/GerarAnalise
        /// Processa a solicitação de análise e retorna o relatório
        /// </summary>
        /// <param name="meses">Quantidade de meses para análise (1-12)</param>
        [HttpPost]
        [ValidateAntiForgeryToken] // Proteção contra CSRF
        public async Task<IActionResult> GerarAnalise(int meses)
        {
            try
            {
                // Validação de entrada
                if (meses < 1 || meses > 12)
                {
                    ModelState.AddModelError("", "Selecione um período entre 1 e 12 meses.");
                    return View("Index");
                }

                // Obtém o ID do usuário autenticado
                var usuarioId = ObterUsuarioId();

                if (usuarioId == 0)
                {
                    TempData["Erro"] = "Não foi possível identificar o usuário. Faça login novamente.";
                    return RedirectToAction("Login", "Usuarios");
                }

                // Chama o service para gerar a análise
                var relatorio = await _tendenciaService.AnalisarTendenciasPorPeriodoAsync(usuarioId, meses);

                // Retorna a view com o relatório
                return View("Resultado", relatorio);
            }
            catch (ArgumentException ex)
            {
                // Erros de validação
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
            catch (Exception ex)
            {
                // Erros inesperados
                TempData["Erro"] = "Erro ao gerar análise. Tente novamente mais tarde.";

                // TODO: Em produção, registrar o erro em log
                // _logger.LogError(ex, "Erro ao gerar análise de tendências");

                return View("Index");
            }
        }

        /// <summary>
        /// Helper method para obter o ID do usuário autenticado
        /// </summary>
        private int ObterUsuarioId()
        {
            // Obtém o ID do usuário das claims do token de autenticação
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int usuarioId))
            {
                return usuarioId;
            }

            return 0; // Retorna 0 se não conseguir obter o ID
        }
    }
}