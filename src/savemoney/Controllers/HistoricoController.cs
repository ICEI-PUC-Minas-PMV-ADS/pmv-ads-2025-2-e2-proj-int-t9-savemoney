// Controllers/HistoricoController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class HistoricoController : Controller
    {
        private readonly AppDbContext _context;

        public HistoricoController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 0;
        }

        // GET: /Historico
        public async Task<IActionResult> Index(int? mes = null, int? ano = null)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // Define o período (padrão: mês atual)
            var hoje = DateTime.Today;
            var mesSelecionado = mes ?? hoje.Month;
            var anoSelecionado = ano ?? hoje.Year;

            var inicioMes = new DateTime(anoSelecionado, mesSelecionado, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            // === RECEITAS (só as recebidas) ===
            var receitas = await _context.Receitas
                .Where(r => r.UsuarioId == userId &&
                            r.Recebido && // só conta se foi recebido
                            r.DataInicio >= inicioMes &&
                            r.DataInicio <= fimMes)
                .Select(r => new MovimentoViewModel
                {
                    Data = r.DataInicio,
                    Descricao = r.Titulo + (r.IsRecurring ? " (recorrente)" : ""),
                    Valor = r.Valor,
                    Tipo = "Entrada",
                    Icone = "bi-arrow-up-circle-fill text-success"
                })
                .ToListAsync();

            // === DESPESAS (só as pagas) ===
            var despesas = await _context.Despesas
                .Where(d => d.UsuarioId == userId &&
                            d.Pago && // só conta se foi paga
                            d.DataInicio >= inicioMes &&
                            d.DataInicio <= fimMes)
                .Select(d => new MovimentoViewModel
                {
                    Data = d.DataInicio,
                    Descricao = d.Titulo + (d.IsRecurring ? " (recorrente)" : ""),
                    Valor = -d.Valor, // negativo para cálculo do saldo
                    Tipo = "Saída",
                    Icone = "bi-arrow-down-circle-fill text-danger"
                })
                .ToListAsync();

            // Junta tudo e ordena por data (mais recente primeiro)
            var movimentos = receitas
                .Concat(despesas)
                .OrderByDescending(m => m.Data)
                .ThenByDescending(m => m.Valor > 0 ? 1 : 0) // entradas primeiro no mesmo dia
                .ToList();

            // Totais do período
            var totalEntradas = receitas.Sum(r => r.Valor);
            var totalSaidas = despesas.Sum(d => -d.Valor);
            var saldoPeriodo = totalEntradas - totalSaidas;

            // Passa dados pra View
            ViewBag.MesAno = $"{inicioMes:MMMM yyyy}";
            ViewBag.TotalEntradas = totalEntradas;
            ViewBag.TotalSaidas = totalSaidas;
            ViewBag.SaldoPeriodo = saldoPeriodo;
            ViewBag.Mes = mesSelecionado;
            ViewBag.Ano = anoSelecionado;

            return View(movimentos);
        }
    }
}