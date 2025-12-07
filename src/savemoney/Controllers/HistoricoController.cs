using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

        // ========================================
        // INDEX - HISTÓRICO COM FILTROS AVANÇADOS
        // ========================================
        public async Task<IActionResult> Index(
            int? mes = null,
            int? ano = null,
            string tipo = "todos",
            string busca = "")
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            var hoje = DateTime.Today;
            var mesSelecionado = mes ?? hoje.Month;
            var anoSelecionado = ano ?? hoje.Year;
            var inicioMes = new DateTime(anoSelecionado, mesSelecionado, 1);
            var fimMes = inicioMes.AddMonths(1).AddDays(-1);

            // Mês anterior (para calcular variação %)
            var mesAnterior = inicioMes.AddMonths(-1);
            var fimMesAnterior = mesAnterior.AddMonths(1).AddDays(-1);

            // Query Receitas
            var receitasQuery = _context.Receitas
                .Where(r => r.UsuarioId == userId && r.Recebido &&
                            r.DataInicio >= inicioMes && r.DataInicio <= fimMes);

            if (!string.IsNullOrWhiteSpace(busca))
            {
                receitasQuery = receitasQuery.Where(r => r.Titulo.Contains(busca));
            }

            var receitas = await receitasQuery
                .Select(r => new MovimentoViewModel
                {
                    Data = r.DataInicio,
                    Descricao = r.Titulo + (r.IsRecurring ? " (recorrente)" : ""),
                    Valor = r.Valor,
                    Tipo = "Entrada",
                    Icone = "arrow_upward"
                })
                .ToListAsync();

            // Query Despesas
            var despesasQuery = _context.Despesas
                .Where(d => d.UsuarioId == userId && d.Pago &&
                            d.DataInicio >= inicioMes && d.DataInicio <= fimMes);

            if (!string.IsNullOrWhiteSpace(busca))
            {
                despesasQuery = despesasQuery.Where(d => d.Titulo.Contains(busca));
            }

            var despesas = await despesasQuery
                .Select(d => new MovimentoViewModel
                {
                    Data = d.DataInicio,
                    Descricao = d.Titulo + (d.IsRecurring ? " (recorrente)" : ""),
                    Valor = -d.Valor,
                    Tipo = "Saída",
                    Icone = "arrow_downward"
                })
                .ToListAsync();

            // Filtra por tipo
            var movimentos = new List<MovimentoViewModel>();

            if (tipo == "receitas")
            {
                movimentos = receitas;
            }
            else if (tipo == "despesas")
            {
                movimentos = despesas;
            }
            else // todos
            {
                movimentos = receitas.Concat(despesas).ToList();
            }

            movimentos = movimentos
                .OrderByDescending(m => m.Data)
                .ThenByDescending(m => Math.Abs(m.Valor))
                .ToList();

            // Totais do mês atual
            var totalEntradas = receitas.Sum(r => r.Valor);
            var totalSaidas = despesas.Sum(d => -d.Valor);
            var saldoPeriodo = totalEntradas - totalSaidas;

            // Totais do mês anterior (para variação %)
            var receitasMesAnterior = await _context.Receitas
                .Where(r => r.UsuarioId == userId && r.Recebido &&
                            r.DataInicio >= mesAnterior && r.DataInicio <= fimMesAnterior)
                .SumAsync(r => (decimal?)r.Valor) ?? 0;

            var despesasMesAnterior = await _context.Despesas
                .Where(d => d.UsuarioId == userId && d.Pago &&
                            d.DataInicio >= mesAnterior && d.DataInicio <= fimMesAnterior)
                .SumAsync(d => (decimal?)d.Valor) ?? 0;

            var saldoMesAnterior = receitasMesAnterior - despesasMesAnterior;

            // Calcula variações %
            var variacaoReceitas = receitasMesAnterior > 0
                ? ((totalEntradas - receitasMesAnterior) / receitasMesAnterior) * 100
                : 0;

            var variacaoDespesas = despesasMesAnterior > 0
                ? ((totalSaidas - despesasMesAnterior) / despesasMesAnterior) * 100
                : 0;

            var variacaoSaldo = saldoMesAnterior != 0
                ? ((saldoPeriodo - saldoMesAnterior) / Math.Abs(saldoMesAnterior)) * 100
                : 0;

            // Gráfico dos últimos 6 meses
            var graficoLabels = new List<string>();
            var graficoReceitas = new List<decimal>();
            var graficoDespesas = new List<decimal>();
            var graficoSaldos = new List<decimal>();

            for (int i = 5; i >= 0; i--)
            {
                var dataRef = hoje.AddMonths(-i);
                var inicio = new DateTime(dataRef.Year, dataRef.Month, 1);
                var fim = inicio.AddMonths(1).AddDays(-1);

                var entrada = await _context.Receitas
                    .Where(r => r.UsuarioId == userId && r.Recebido &&
                                r.DataInicio >= inicio && r.DataInicio <= fim)
                    .SumAsync(r => (decimal?)r.Valor) ?? 0;

                var saida = await _context.Despesas
                    .Where(d => d.UsuarioId == userId && d.Pago &&
                                d.DataInicio >= inicio && d.DataInicio <= fim)
                    .SumAsync(d => (decimal?)d.Valor) ?? 0;

                graficoLabels.Add(dataRef.ToString("MMM/yy"));
                graficoReceitas.Add(entrada);
                graficoDespesas.Add(saida);
                graficoSaldos.Add(entrada - saida);
            }

            // ViewBag
            ViewBag.GraficoLabels = graficoLabels;
            ViewBag.GraficoReceitas = graficoReceitas;
            ViewBag.GraficoDespesas = graficoDespesas;
            ViewBag.GraficoSaldos = graficoSaldos;
            ViewBag.MesAno = inicioMes.ToString("MMMM yyyy");
            ViewBag.MesAnoCompleto = inicioMes.ToString("MMMM 'de' yyyy");
            ViewBag.TotalEntradas = totalEntradas;
            ViewBag.TotalSaidas = totalSaidas;
            ViewBag.SaldoPeriodo = saldoPeriodo;
            ViewBag.VariacaoReceitas = variacaoReceitas;
            ViewBag.VariacaoDespesas = variacaoDespesas;
            ViewBag.VariacaoSaldo = variacaoSaldo;
            ViewBag.Mes = mesSelecionado;
            ViewBag.Ano = anoSelecionado;
            ViewBag.TipoFiltro = tipo;
            ViewBag.BuscaFiltro = busca;
            ViewBag.QuantidadeMovimentos = movimentos.Count;

            return View(movimentos);
        }

        // ========================================
        // EXPORTAR PDF
        // ========================================
        [HttpGet]
        public async Task<IActionResult> ExportarPdf(int mes, int ano)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var dataRef = new DateTime(ano, mes, 1);
            var inicioMes = dataRef;
            var fimMes = dataRef.AddMonths(1).AddDays(-1);

            var receitas = await _context.Receitas
                .Where(r => r.UsuarioId == userId && r.Recebido &&
                            r.DataInicio >= inicioMes && r.DataInicio <= fimMes)
                .OrderByDescending(r => r.DataInicio)
                .ToListAsync();

            var despesas = await _context.Despesas
                .Where(d => d.UsuarioId == userId && d.Pago &&
                            d.DataInicio >= inicioMes && d.DataInicio <= fimMes)
                .OrderByDescending(d => d.DataInicio)
                .ToListAsync();

            var totalEntradas = receitas.Sum(r => r.Valor);
            var totalSaidas = despesas.Sum(d => d.Valor);
            var saldoFinal = totalEntradas - totalSaidas;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Histórico Financeiro - {dataRef:MMMM yyyy}")
                        .FontSize(24)
                        .Bold()
                        .FontColor("#10111a")
                        .AlignCenter();

                    page.Content().PaddingVertical(2, Unit.Centimetre).Column(col =>
                    {
                        // Resumo
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Padding(10).Border(2).BorderColor(Colors.Green.Accent3)
                                .Text($"Entradas\n{totalEntradas:C}").FontSize(16).Bold().AlignCenter();
                            row.RelativeItem().Padding(10).Border(2).BorderColor(Colors.Red.Accent3)
                                .Text($"Saídas\n{totalSaidas:C}").FontSize(16).Bold().AlignCenter();
                            row.RelativeItem().Padding(10).Border(2).BorderColor(saldoFinal >= 0 ? Colors.Blue.Accent3 : Colors.Orange.Accent3)
                                .Text($"Saldo\n{saldoFinal:C}").FontSize(18).Bold().AlignCenter();
                        });

                        col.Item().PaddingTop(30).Text("Movimentações").FontSize(18).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(90);
                                columns.RelativeColumn(3);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(70);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background("#10111a").Padding(8).Text("Data").FontColor(Colors.White).Bold();
                                header.Cell().Background("#10111a").Padding(8).Text("Descrição").FontColor(Colors.White).Bold();
                                header.Cell().Background("#10111a").Padding(8).Text("Valor").FontColor(Colors.White).Bold().AlignRight();
                                header.Cell().Background("#10111a").Padding(8).Text("Tipo").FontColor(Colors.White).Bold();
                            });

                            foreach (var r in receitas)
                            {
                                table.Cell().Padding(6).Text($"{r.DataInicio:dd/MM/yyyy}");
                                table.Cell().Padding(6).Text(r.Titulo + (r.IsRecurring ? " (recorrente)" : ""));
                                table.Cell().Padding(6).Text($"{r.Valor:C}").FontColor(Colors.Green.Medium).Bold().AlignRight();
                                table.Cell().Padding(6).Background(Colors.Green.Lighten4).Text("Entrada").AlignCenter();
                            }

                            foreach (var d in despesas)
                            {
                                table.Cell().Padding(6).Text($"{d.DataInicio:dd/MM/yyyy}");
                                table.Cell().Padding(6).Text(d.Titulo + (d.IsRecurring ? " (recorrente)" : ""));
                                table.Cell().Padding(6).Text($"{d.Valor:C}").FontColor(Colors.Red.Medium).Bold().AlignRight();
                                table.Cell().Padding(6).Background(Colors.Red.Lighten4).Text("Saída").AlignCenter();
                            }
                        });

                        col.Item().AlignCenter().PaddingTop(40)
                            .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            var nomeArquivo = $"Historico_{dataRef:MMMM_yyyy}.pdf";

            return File(pdfBytes, "application/pdf", nomeArquivo);
        }
    }
}