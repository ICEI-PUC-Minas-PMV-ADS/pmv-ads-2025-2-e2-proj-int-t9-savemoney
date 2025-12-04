using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models.Enums;
using savemoney.Models.ViewModels;
using savemoney.Services;
using savemoney.Services.Interfaces;
using System.Security.Claims;

namespace savemoney.Controllers
{
    /// <summary>
    /// Controller responsável pelo DRE Gerencial Simplificado.
    /// Funcionalidade do módulo PJ (R7).
    /// </summary>
    [Authorize]
    public class DreGerencialController : Controller
    {
        private readonly IDreGerencialService _dreService;

        public DreGerencialController(IDreGerencialService dreService)
        {
            _dreService = dreService;
        }

        /// <summary>
        /// Exibe a tela do DRE Gerencial com dados do mês atual.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            // Período padrão: mês atual
            var dataInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1);

            var viewModel = await _dreService.GerarDreAsync(
                usuarioId,
                dataInicio,
                dataFim,
                TipoRegimeDre.Competencia);

            // Carrega histórico para gráfico
            viewModel.HistoricoMensal = await _dreService.ObterHistoricoMensalAsync(
                usuarioId,
                meses: 6,
                TipoRegimeDre.Competencia);

            return View(viewModel);
        }

        /// <summary>
        /// Aplica filtros e retorna o DRE atualizado.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DreGerencialViewModel filtros)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return RedirectToAction("Login", "Usuarios");

            DreGerencialViewModel viewModel;

            // Verifica se tem comparativo habilitado
            if (filtros.HabilitarComparativo &&
                filtros.DataInicioComparativo.HasValue &&
                filtros.DataFimComparativo.HasValue)
            {
                viewModel = await _dreService.GerarDreComparativoAsync(
                    usuarioId,
                    filtros.DataInicio,
                    filtros.DataFim,
                    filtros.DataInicioComparativo.Value,
                    filtros.DataFimComparativo.Value,
                    filtros.Regime);
            }
            else
            {
                viewModel = await _dreService.GerarDreAsync(
                    usuarioId,
                    filtros.DataInicio,
                    filtros.DataFim,
                    filtros.Regime);
            }

            // Carrega histórico para gráfico
            viewModel.HistoricoMensal = await _dreService.ObterHistoricoMensalAsync(
                usuarioId,
                meses: 6,
                filtros.Regime);

            return View(viewModel);
        }

        /// <summary>
        /// Retorna dados do DRE em JSON para atualização via AJAX.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterDados(
            DateTime dataInicio,
            DateTime dataFim,
            TipoRegimeDre regime = TipoRegimeDre.Competencia)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return Unauthorized();

            var viewModel = await _dreService.GerarDreAsync(
                usuarioId,
                dataInicio,
                dataFim,
                regime);

            return Json(new
            {
                success = true,
                dados = new
                {
                    receitaBruta = viewModel.PeriodoAtual.ReceitaBruta,
                    custosVariaveis = viewModel.PeriodoAtual.CustosVariaveis,
                    margemContribuicao = viewModel.PeriodoAtual.MargemContribuicao,
                    margemContribuicaoPercentual = viewModel.PeriodoAtual.MargemContribuicaoPercentual,
                    despesasOperacionais = viewModel.PeriodoAtual.DespesasOperacionais,
                    lucroLiquido = viewModel.PeriodoAtual.LucroLiquido,
                    margemLiquidaPercentual = viewModel.PeriodoAtual.MargemLiquidaPercentual,
                    temLucro = viewModel.PeriodoAtual.TemLucro,
                    receitasDetalhadas = viewModel.PeriodoAtual.ReceitasDetalhadas,
                    custosDetalhados = viewModel.PeriodoAtual.CustosDetalhados,
                    despesasDetalhadas = viewModel.PeriodoAtual.DespesasDetalhadas
                }
            });
        }

        /// <summary>
        /// Retorna histórico mensal em JSON para gráficos.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterHistorico(
            int meses = 6,
            TipoRegimeDre regime = TipoRegimeDre.Competencia)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == 0)
                return Unauthorized();

            var historico = await _dreService.ObterHistoricoMensalAsync(
                usuarioId,
                meses,
                regime);

            return Json(new
            {
                success = true,
                dados = historico.Select(h => new
                {
                    mesAno = h.MesAnoAbreviado,
                    receitaBruta = h.ReceitaBruta,
                    custosVariaveis = h.CustosVariaveis,
                    margemContribuicao = h.MargemContribuicao,
                    despesasOperacionais = h.DespesasOperacionais,
                    lucroLiquido = h.LucroLiquido
                })
            });
        }

        /// <summary>
        /// Obtém o ID do usuário autenticado.
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