using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models.ViewModels;
using savemoney.Services.Interfaces;
using System.Security.Claims;

namespace savemoney.Controllers
{
    /// <summary>
    /// Controller responsavel pelos KPIs Corporativos.
    /// Funcionalidade do modulo PJ (R7).
    /// </summary>
    [Authorize]
    public class KpisCorporativosController : Controller
    {
        private readonly IKpisCorporativosService _kpisService;

        public KpisCorporativosController(IKpisCorporativosService kpisService)
        {
            _kpisService = kpisService;
        }

        /// <summary>
        /// Exibe a tela de KPIs Corporativos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            // Periodo padrao: mes atual
            var dataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1);

            var viewModel = await _kpisService.CalcularKpisAsync(
                usuarioId,
                dataInicio,
                dataFim);

            return View(viewModel);
        }

        /// <summary>
        /// Aplica filtros e retorna KPIs atualizados.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(KpisCorporativosViewModel filtros)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            var viewModel = await _kpisService.CalcularKpisAsync(
                usuarioId,
                filtros.DataInicio,
                filtros.DataFim);

            return View(viewModel);
        }

        /// <summary>
        /// Retorna KPIs em JSON para atualizacao via AJAX.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterKpis(DateTime dataInicio, DateTime dataFim)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return Unauthorized();

            var viewModel = await _kpisService.CalcularKpisAsync(
                usuarioId,
                dataInicio,
                dataFim);

            return Json(new
            {
                success = true,
                dados = new
                {
                    // Margem de Lucro
                    receitaTotal = viewModel.ReceitaTotal,
                    despesaTotal = viewModel.DespesaTotal,
                    lucroBruto = viewModel.LucroBruto,
                    margemLucro = viewModel.MargemLucro,
                    temLucro = viewModel.TemLucro,

                    // Burn Rate
                    custosFixosMensal = viewModel.CustosFixosMensal,
                    despesasOperacionaisMensal = viewModel.DespesasOperacionaisMensal,
                    burnRate = viewModel.BurnRate,
                    saldoDisponivel = viewModel.SaldoDisponivel,
                    runwayMeses = viewModel.RunwayMeses,

                    // Ponto de Equilibrio
                    custosVariaveis = viewModel.CustosVariaveis,
                    margemContribuicaoPercentual = viewModel.MargemContribuicaoPercentual,
                    pontoEquilibrio = viewModel.PontoEquilibrio,
                    percentualAtingido = viewModel.PercentualAtingido,
                    atingiuEquilibrio = viewModel.AtingiuEquilibrio
                }
            });
        }

        /// <summary>
        /// Retorna historico mensal em JSON para graficos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterHistorico(int meses = 6)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return Unauthorized();

            var historico = await _kpisService.ObterHistoricoMensalAsync(usuarioId, meses);

            return Json(new
            {
                success = true,
                dados = historico.Select(h => new
                {
                    mesAno = h.MesAnoAbreviado,
                    receita = h.Receita,
                    despesa = h.Despesa,
                    lucro = h.Lucro,
                    margemLucro = h.MargemLucro
                })
            });
        }

        /// <summary>
        /// Obtem o ID do usuario autenticado.
        /// </summary>
        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            // Fallback: tenta obter da Session
            var sessionUserId = HttpContext.Session.GetInt32("UsuarioId");
            return sessionUserId ?? 0;
        }
    }
}