using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using SaveMoney.Models.ViewModels;
using System.Security.Claims;

namespace SaveMoney.Controllers
{
    [Authorize]
    public class ProjecaoFinanceiraController : Controller
    {
        private readonly AppDbContext _context;

        public ProjecaoFinanceiraController(AppDbContext context)
        {
            _context = context;
        }

        // Parâmetro 'meses' permite o filtro dinâmico (padrão 6)
        public async Task<IActionResult> Index(int meses = 6)
        {
            // Garante limite entre 1 e 12 meses para segurança
            if (meses < 1) meses = 1;
            if (meses > 12) meses = 12;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Usuarios");
            var userId = int.Parse(userIdClaim);

            // ==========================================
            // 1. SALDO REAL HOJE (O que tem na conta)
            // ==========================================
            var totalRecebido = await _context.Receitas
                .Where(r => r.UsuarioId == userId && r.Recebido)
                .SumAsync(r => r.Valor);

            var totalPago = await _context.Despesas
                .Where(d => d.UsuarioId == userId && d.Pago)
                .SumAsync(d => d.Valor);

            var saldoAtual = totalRecebido - totalPago;

            // ==========================================
            // 2. FLUXO RECORRENTE (Para o Futuro)
            // ==========================================
            // Filtra apenas itens marcados como RECORRENTES e Mensais

            var receitasRecorrentes = await _context.Receitas
                .Where(r => r.UsuarioId == userId
                         && r.IsRecurring
                         && r.Recurrence == Receita.RecurrenceType.Monthly)
                .SumAsync(r => r.Valor);

            var despesasRecorrentes = await _context.Despesas
                .Where(d => d.UsuarioId == userId
                         && d.IsRecurring) // Assumindo recorrencia padrão, ajuste se tiver Enum
                .SumAsync(d => d.Valor);

            var fluxoCaixaMensal = receitasRecorrentes - despesasRecorrentes;

            // ==========================================
            // 3. MONTAR VIEWMODEL
            // ==========================================
            var model = new ProjecaoViewModel
            {
                SaldoAtual = saldoAtual,
                FluxoMensal = fluxoCaixaMensal,
                Meses = new List<string>(),
                Saldos = new List<decimal>()
            };

            // Ponto 0 (Hoje)
            model.Meses.Add("Hoje");
            model.Saldos.Add(saldoAtual);

            var saldoProjetado = saldoAtual;
            var dataReferencia = DateTime.Today;

            // Loop dinâmico baseado na escolha do usuário
            for (int i = 1; i <= meses; i++)
            {
                dataReferencia = dataReferencia.AddMonths(1);
                saldoProjetado += fluxoCaixaMensal;

                model.Meses.Add(dataReferencia.ToString("MMM/yy"));
                model.Saldos.Add(saldoProjetado);
            }

            model.SaldoProjetado6Meses = saldoProjetado;

            // ViewBag para o Dropdown da tela funcionar corretamente
            ViewBag.MesesSelecionados = meses;

            return View(model);
        }
    }
}