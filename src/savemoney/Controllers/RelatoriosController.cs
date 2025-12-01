using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Models.ViewModels;
using savemoney.Services;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace savemoney.Controllers
{
    [Authorize]
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? dataInicio,
            DateTime? dataFim,
            PeriodoAgregacao? agregacao,
            string? tipoRecorrencia,
            string? categoria)
        {
            // 1. Identificar Usuário Logado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Usuarios");
            var userId = int.Parse(userIdClaim);

            // 2. Definir Período (Padrão: Início do mês atual até hoje)
            // Pega do dia de hoje até 1 mês atrás por padrão (ex: 23/11 a 23/12)
            var inicio = dataInicio ?? DateTime.Today.AddMonths(-1);
            var fim = dataFim ?? DateTime.Today;

            // Se o usuário filtrar uma data inicio maior que fim, ajusta para 30 dias antes
            if (inicio > fim) inicio = fim.AddDays(-30);


            var periodoAgregacao = agregacao ?? PeriodoAgregacao.Mensal;

            // Garantir que inicio não seja maior que fim
            if (inicio > fim) inicio = fim.AddDays(-30);

            // 3. Buscar Dados Otimizados (SELECT apenas colunas necessárias)
            var receitasBrutas = await _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.Titulo,
                    r.Valor,
                    r.DataInicio,
                    r.DataFim,
                    r.IsRecurring,
                    r.Recurrence,
                    r.RecurrenceCount
                })
                .ToListAsync();

            var despesasBrutas = await _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == userId)
                .Select(d => new
                {
                    d.Id,
                    d.Titulo,
                    d.Valor,
                    d.DataInicio,
                    d.DataFim,
                    d.IsRecurring,
                    d.Recurrence,
                    d.RecurrenceCount,
                    CategoriaNome = d.BudgetCategory != null && d.BudgetCategory.Category != null
                        ? d.BudgetCategory.Category.Name
                        : "Sem Categoria"
                })
                .ToListAsync();

            // Converter para objetos compatíveis com o serviço
            var receitas = receitasBrutas.Select(r => new ReceitaDTO
            {
                Titulo = r.Titulo,
                Valor = r.Valor,
                DataInicio = r.DataInicio,
                DataFim = r.DataFim,
                IsRecurring = r.IsRecurring,
                Recurrence = r.Recurrence.ToString(),
                RecurrenceCount = r.RecurrenceCount
            }).ToList();

            var despesas = despesasBrutas.Select(d => new DespesaDTO
            {
                Titulo = d.Titulo,
                Valor = d.Valor,
                DataInicio = d.DataInicio,
                DataFim = d.DataFim,
                IsRecurring = d.IsRecurring,
                Recurrence = d.Recurrence.ToString(),
                RecurrenceCount = d.RecurrenceCount,
                CategoriaNome = d.CategoriaNome
            }).ToList();

            // 4. Aplicar Filtro de Tipo de Recorrência
            if (!string.IsNullOrEmpty(tipoRecorrencia))
            {
                if (tipoRecorrencia == "fixo")
                {
                    receitas = receitas.Where(r => r.IsRecurring).ToList();
                    despesas = despesas.Where(d => d.IsRecurring).ToList();
                }
                else if (tipoRecorrencia == "variavel")
                {
                    receitas = receitas.Where(r => !r.IsRecurring).ToList();
                    despesas = despesas.Where(d => !d.IsRecurring).ToList();
                }
            }

            // 5. Processar Recorrências (Período Atual)
            var transacoesProcessadas = ServicoCalculoRecorrencia.ProcessarTransacoes(
                receitas, despesas, inicio, fim
            );

            // 6. Aplicar Filtro de Categoria
            if (!string.IsNullOrEmpty(categoria))
            {
                transacoesProcessadas = transacoesProcessadas
                    .Where(t => t.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // 7. Calcular KPIs do Período Atual
            var totalReceitas = transacoesProcessadas.Where(t => t.EhReceita).Sum(t => t.Valor);
            var totalDespesas = transacoesProcessadas.Where(t => !t.EhReceita).Sum(t => t.Valor);
            var saldo = totalReceitas - totalDespesas;
            var taxaPoupanca = totalReceitas > 0 ? (saldo / totalReceitas) * 100 : 0;

            // 8. Calcular Período Anterior para Comparação
            var diasPeriodo = (fim - inicio).Days;
            var inicioAnterior = inicio.AddDays(-diasPeriodo - 1);
            var fimAnterior = inicio.AddDays(-1);

            var transacoesAnterior = ServicoCalculoRecorrencia.ProcessarTransacoes(
                receitas, despesas, inicioAnterior, fimAnterior
            );

            var receitasAnterior = transacoesAnterior.Where(t => t.EhReceita).Sum(t => t.Valor);
            var despesasAnterior = transacoesAnterior.Where(t => !t.EhReceita).Sum(t => t.Valor);
            var saldoAnterior = receitasAnterior - despesasAnterior;

            // 9. Calcular Variações Percentuais
            var variacaoReceitas = CalcularVariacao(totalReceitas, receitasAnterior);
            var variacaoDespesas = CalcularVariacao(totalDespesas, despesasAnterior);
            var variacaoSaldo = CalcularVariacao(saldo, saldoAnterior);

            // 10. Identificar Categoria Vilã e Heroína
            var analiseCategoria = ServicoCalculoRecorrencia.AnalisarCategorias(
                transacoesProcessadas.Where(t => !t.EhReceita).ToList(),
                transacoesAnterior.Where(t => !t.EhReceita).ToList()
            );

            // 11. Calcular Projeção Futura (6 meses)
            var projecaoFutura = ServicoCalculoRecorrencia.CalcularProjecaoFutura(
                receitas, despesas, saldo
            );

            // 12. Preparar Dados para Gráfico de Fluxo (Timeline)
            var (labelsDatas, valoresReceitas, valoresDespesas) = PrepararDadosTimeline(
                transacoesProcessadas, inicio, fim, periodoAgregacao);

            // 13. Preparar Dados para Gráfico de Categorias (Top Gastos)
            var gastosPorCategoria = transacoesProcessadas
                .Where(t => !t.EhReceita)
                .GroupBy(t => t.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                .OrderByDescending(x => x.Total)
                .Take(7)
                .ToList();

            // 14. Preparar Top Receitas
            var topReceitas = transacoesProcessadas
                .Where(t => t.EhReceita)
                .GroupBy(t => t.Descricao)
                .Select(g => new TopItemViewModel
                {
                    Nome = g.Key,
                    Valor = g.Sum(x => x.Valor),
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Valor)
                .Take(5)
                .ToList();

            // 15. Preparar Top Despesas
            var topDespesas = transacoesProcessadas
                .Where(t => !t.EhReceita)
                .GroupBy(t => t.Descricao)
                .Select(g => new TopItemViewModel
                {
                    Nome = g.Key,
                    Valor = g.Sum(x => x.Valor),
                    Quantidade = g.Count(),
                    Categoria = transacoesProcessadas.FirstOrDefault(t => t.Descricao == g.Key)?.Categoria ?? "Sem Categoria"
                })
                .OrderByDescending(x => x.Valor)
                .Take(5)
                .ToList();

            // 16. Análise por Dia da Semana
            var gastosPorDiaSemana = transacoesProcessadas
                .Where(t => !t.EhReceita)
                .GroupBy(t => t.DiaSemana)
                .Select(g => new { Dia = g.Key, Total = g.Sum(x => x.Valor) })
                .OrderBy(x => ObterOrdemDiaSemana(x.Dia))
                .ToList();

            // 17. Gerar Diagnósticos Inteligentes
            var diagnosticos = GerarDiagnosticos(
                totalReceitas, totalDespesas, taxaPoupanca,
                gastosPorCategoria.Select(x => (x.Categoria, x.Total)).ToList(),
                analiseCategoria,
                gastosPorDiaSemana.Select(x => (x.Dia, x.Total)).ToList()
            );

            // 18. Obter Lista de Categorias para Filtro
            var categoriasDisponiveis = transacoesProcessadas
                .Select(t => t.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            // 19. Montar ViewModel Completo
            var viewModel = new RelatorioFinanceiroViewModel
            {
                // Filtros
                DataInicio = inicio,
                DataFim = fim,
                Agregacao = periodoAgregacao,
                TipoRecorrenciaFiltro = tipoRecorrencia,
                CategoriaFiltro = categoria,
                CategoriasDisponiveis = categoriasDisponiveis,

                // KPIs Atuais
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = saldo,
                TaxaPoupanca = taxaPoupanca,

                // KPIs Anteriores e Variações
                ReceitasAnterior = receitasAnterior,
                DespesasAnterior = despesasAnterior,
                SaldoAnterior = saldoAnterior,
                VariacaoReceitas = variacaoReceitas,
                VariacaoDespesas = variacaoDespesas,
                VariacaoSaldo = variacaoSaldo,

                // Análise de Categorias
                CategoriaVila = analiseCategoria.CategoriaVila,
                CategoriaVilaVariacao = analiseCategoria.VilaVariacao,
                CategoriaHeroina = analiseCategoria.CategoriaHeroina,
                CategoriaHeroinaVariacao = analiseCategoria.HeroinaVariacao,

                // Projeção Futura
                ProjecaoMeses = projecaoFutura.Meses,
                ProjecaoSaldos = projecaoFutura.Saldos,
                JsonProjecaoMeses = JsonSerializer.Serialize(projecaoFutura.Meses),
                JsonProjecaoSaldos = JsonSerializer.Serialize(projecaoFutura.Saldos),

                // Listas
                TransacoesDetalhadas = transacoesProcessadas,
                TopReceitas = topReceitas,
                TopDespesas = topDespesas,
                Diagnosticos = diagnosticos,

                // Análise por Dia da Semana
                JsonDiasSemana = JsonSerializer.Serialize(gastosPorDiaSemana.Select(x => x.Dia)),
                JsonValoresDiaSemana = JsonSerializer.Serialize(gastosPorDiaSemana.Select(x => x.Total)),

                // Dados JSON para Gráficos
                JsonLabelsTimeline = JsonSerializer.Serialize(labelsDatas),
                JsonDadosReceitas = JsonSerializer.Serialize(valoresReceitas),
                JsonDadosDespesas = JsonSerializer.Serialize(valoresDespesas),
                JsonLabelsCategorias = JsonSerializer.Serialize(gastosPorCategoria.Select(x => x.Categoria)),
                JsonValoresCategorias = JsonSerializer.Serialize(gastosPorCategoria.Select(x => x.Total)),

                // Controle de Estado
                TemDadosSuficientes = transacoesProcessadas.Any(),
                QuantidadeTransacoes = transacoesProcessadas.Count
            };

            return View(viewModel);
        }

        /// <summary>
        /// Exporta os dados do relatório para CSV
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExportarCsv(DateTime? dataInicio, DateTime? dataFim, string? tipoRecorrencia)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            var inicio = dataInicio ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fim = dataFim ?? DateTime.Now;

            // Buscar dados
            var receitasBrutas = await _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == userId)
                .Select(r => new ReceitaDTO
                {
                    Titulo = r.Titulo,
                    Valor = r.Valor,
                    DataInicio = r.DataInicio,
                    DataFim = r.DataFim,
                    IsRecurring = r.IsRecurring,
                    Recurrence = r.Recurrence.ToString(),
                    RecurrenceCount = r.RecurrenceCount
                })
                .ToListAsync();

            var despesasBrutas = await _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == userId)
                .Select(d => new DespesaDTO
                {
                    Titulo = d.Titulo,
                    Valor = d.Valor,
                    DataInicio = d.DataInicio,
                    DataFim = d.DataFim,
                    IsRecurring = d.IsRecurring,
                    Recurrence = d.Recurrence.ToString(),
                    RecurrenceCount = d.RecurrenceCount,
                    CategoriaNome = d.BudgetCategory != null && d.BudgetCategory.Category != null
                        ? d.BudgetCategory.Category.Name
                        : "Sem Categoria"
                })
                .ToListAsync();

            // Aplicar filtro de recorrência
            if (!string.IsNullOrEmpty(tipoRecorrencia))
            {
                if (tipoRecorrencia == "fixo")
                {
                    receitasBrutas = receitasBrutas.Where(r => r.IsRecurring).ToList();
                    despesasBrutas = despesasBrutas.Where(d => d.IsRecurring).ToList();
                }
                else if (tipoRecorrencia == "variavel")
                {
                    receitasBrutas = receitasBrutas.Where(r => !r.IsRecurring).ToList();
                    despesasBrutas = despesasBrutas.Where(d => !d.IsRecurring).ToList();
                }
            }

            var transacoes = ServicoCalculoRecorrencia.ProcessarTransacoes(receitasBrutas, despesasBrutas, inicio, fim);

            // Construir CSV
            var sb = new StringBuilder();
            sb.AppendLine("Data;Tipo;Descrição;Categoria;Valor;Dia da Semana");

            foreach (var t in transacoes.OrderBy(x => x.Data))
            {
                var tipo = t.EhReceita ? "Receita" : "Despesa";
                var valor = t.EhReceita ? t.Valor : -t.Valor;
                sb.AppendLine($"{t.Data:dd/MM/yyyy};{tipo};{EscapeCsv(t.Descricao)};{EscapeCsv(t.Categoria)};{valor.ToString("F2", CultureInfo.GetCultureInfo("pt-BR"))};{t.DiaSemana}");
            }

            // Adicionar resumo
            sb.AppendLine();
            sb.AppendLine("=== RESUMO ===");
            sb.AppendLine($"Período;{inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}");
            sb.AppendLine($"Total Receitas;{transacoes.Where(t => t.EhReceita).Sum(t => t.Valor).ToString("F2", CultureInfo.GetCultureInfo("pt-BR"))}");
            sb.AppendLine($"Total Despesas;{transacoes.Where(t => !t.EhReceita).Sum(t => t.Valor).ToString("F2", CultureInfo.GetCultureInfo("pt-BR"))}");
            sb.AppendLine($"Saldo;{(transacoes.Where(t => t.EhReceita).Sum(t => t.Valor) - transacoes.Where(t => !t.EhReceita).Sum(t => t.Valor)).ToString("F2", CultureInfo.GetCultureInfo("pt-BR"))}");

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            var nomeArquivo = $"relatorio_financeiro_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.csv";

            return File(bytes, "text/csv; charset=utf-8", nomeArquivo);
        }

        /// <summary>
        /// Retorna dados filtrados via AJAX para drill-down
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> FiltrarPorCategoria(string categoria, DateTime dataInicio, DateTime dataFim)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            var despesas = await _context.Despesas
                .AsNoTracking()
                .Where(d => d.UsuarioId == userId)
                .Select(d => new DespesaDTO
                {
                    Titulo = d.Titulo,
                    Valor = d.Valor,
                    DataInicio = d.DataInicio,
                    DataFim = d.DataFim,
                    IsRecurring = d.IsRecurring,
                    Recurrence = d.Recurrence.ToString(),
                    RecurrenceCount = d.RecurrenceCount,
                    CategoriaNome = d.BudgetCategory != null && d.BudgetCategory.Category != null
                        ? d.BudgetCategory.Category.Name
                        : "Sem Categoria"
                })
                .ToListAsync();

            var transacoes = ServicoCalculoRecorrencia.ProcessarTransacoes(
                new List<ReceitaDTO>(), despesas, dataInicio, dataFim
            );

            var filtradas = transacoes
                .Where(t => t.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                .Select(t => new
                {
                    t.Data,
                    DataFormatada = t.Data.ToString("dd/MM/yyyy"),
                    t.Descricao,
                    t.Valor,
                    ValorFormatado = t.Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR")),
                    t.DiaSemana,
                    t.ClasseCssStatus
                })
                .OrderByDescending(t => t.Data)
                .ToList();

            return Json(filtradas);
        }

        #region Métodos Auxiliares

        private static decimal CalcularVariacao(decimal atual, decimal anterior)
        {
            if (anterior == 0) return atual > 0 ? 100 : 0;
            return ((atual - anterior) / Math.Abs(anterior)) * 100;
        }

        private static (List<string> Labels, List<decimal> Receitas, List<decimal> Despesas) PrepararDadosTimeline(
            List<TransacaoProcessada> transacoes,
            DateTime inicio,
            DateTime fim,
            PeriodoAgregacao agregacao)
        {
            var labels = new List<string>();
            var receitas = new List<decimal>();
            var despesas = new List<decimal>();

            switch (agregacao)
            {
                case PeriodoAgregacao.Diario:
                    for (var data = inicio.Date; data <= fim.Date; data = data.AddDays(1))
                    {
                        labels.Add(data.ToString("dd/MM"));
                        var transacoesDia = transacoes.Where(t => t.Data.Date == data);
                        receitas.Add(transacoesDia.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesDia.Where(t => !t.EhReceita).Sum(t => t.Valor));
                    }
                    break;

                case PeriodoAgregacao.Semanal:
                    var inicioSemana = inicio.Date;
                    while (inicioSemana <= fim.Date)
                    {
                        var fimSemana = inicioSemana.AddDays(6);
                        if (fimSemana > fim.Date) fimSemana = fim.Date;

                        labels.Add($"{inicioSemana:dd/MM}");
                        var transacoesSemana = transacoes.Where(t => t.Data.Date >= inicioSemana && t.Data.Date <= fimSemana);
                        receitas.Add(transacoesSemana.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesSemana.Where(t => !t.EhReceita).Sum(t => t.Valor));

                        inicioSemana = inicioSemana.AddDays(7);
                    }
                    break;

                case PeriodoAgregacao.Mensal:
                    var mesAtual = new DateTime(inicio.Year, inicio.Month, 1);
                    while (mesAtual <= fim)
                    {
                        var fimMes = mesAtual.AddMonths(1).AddDays(-1);
                        if (fimMes > fim) fimMes = fim;

                        labels.Add(mesAtual.ToString("MMM/yy", CultureInfo.GetCultureInfo("pt-BR")));
                        var transacoesMes = transacoes.Where(t => t.Data.Date >= mesAtual && t.Data.Date <= fimMes);
                        receitas.Add(transacoesMes.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesMes.Where(t => !t.EhReceita).Sum(t => t.Valor));

                        mesAtual = mesAtual.AddMonths(1);
                    }
                    break;

                case PeriodoAgregacao.Trimestral:
                    var trimestreAtual = new DateTime(inicio.Year, ((inicio.Month - 1) / 3) * 3 + 1, 1);
                    while (trimestreAtual <= fim)
                    {
                        var fimTrimestre = trimestreAtual.AddMonths(3).AddDays(-1);
                        if (fimTrimestre > fim) fimTrimestre = fim;

                        var trimestre = ((trimestreAtual.Month - 1) / 3) + 1;
                        labels.Add($"T{trimestre}/{trimestreAtual:yy}");
                        var transacoesTrimestre = transacoes.Where(t => t.Data.Date >= trimestreAtual && t.Data.Date <= fimTrimestre);
                        receitas.Add(transacoesTrimestre.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesTrimestre.Where(t => !t.EhReceita).Sum(t => t.Valor));

                        trimestreAtual = trimestreAtual.AddMonths(3);
                    }
                    break;

                case PeriodoAgregacao.Semestral:
                    var semestreAtual = new DateTime(inicio.Year, inicio.Month <= 6 ? 1 : 7, 1);
                    while (semestreAtual <= fim)
                    {
                        var fimSemestre = semestreAtual.AddMonths(6).AddDays(-1);
                        if (fimSemestre > fim) fimSemestre = fim;

                        var semestre = semestreAtual.Month <= 6 ? 1 : 2;
                        labels.Add($"S{semestre}/{semestreAtual:yy}");
                        var transacoesSemestre = transacoes.Where(t => t.Data.Date >= semestreAtual && t.Data.Date <= fimSemestre);
                        receitas.Add(transacoesSemestre.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesSemestre.Where(t => !t.EhReceita).Sum(t => t.Valor));

                        semestreAtual = semestreAtual.AddMonths(6);
                    }
                    break;

                case PeriodoAgregacao.Anual:
                    var anoAtual = new DateTime(inicio.Year, 1, 1);
                    while (anoAtual <= fim)
                    {
                        var fimAno = new DateTime(anoAtual.Year, 12, 31);
                        if (fimAno > fim) fimAno = fim;

                        labels.Add(anoAtual.ToString("yyyy"));
                        var transacoesAno = transacoes.Where(t => t.Data.Date >= anoAtual && t.Data.Date <= fimAno);
                        receitas.Add(transacoesAno.Where(t => t.EhReceita).Sum(t => t.Valor));
                        despesas.Add(transacoesAno.Where(t => !t.EhReceita).Sum(t => t.Valor));

                        anoAtual = anoAtual.AddYears(1);
                    }
                    break;
            }

            return (labels, receitas, despesas);
        }

        private static List<DiagnosticoItem> GerarDiagnosticos(
            decimal totalReceitas,
            decimal totalDespesas,
            decimal taxaPoupanca,
            List<(string Categoria, decimal Total)> gastosPorCategoria,
            AnaliseCategoriaResult analiseCategoria,
            List<(string Dia, decimal Total)> gastosPorDia)
        {
            var diagnosticos = new List<DiagnosticoItem>();

            // Análise de Saldo
            if (totalDespesas > totalReceitas)
            {
                diagnosticos.Add(new DiagnosticoItem
                {
                    Icone = "warning",
                    Tipo = "danger",
                    Titulo = "Saldo Negativo",
                    Mensagem = $"Você gastou R$ {(totalDespesas - totalReceitas):N2} a mais do que ganhou neste período."
                });
            }

            // Taxa de Poupança
            if (taxaPoupanca >= 20)
            {
                diagnosticos.Add(new DiagnosticoItem
                {
                    Icone = "check_circle",
                    Tipo = "success",
                    Titulo = "Excelente Poupança",
                    Mensagem = $"Parabéns! Você está poupando {taxaPoupanca:F1}% da sua renda."
                });
            }
            else if (taxaPoupanca > 0 && taxaPoupanca < 10)
            {
                diagnosticos.Add(new DiagnosticoItem
                {
                    Icone = "info",
                    Tipo = "warning",
                    Titulo = "Margem Apertada",
                    Mensagem = $"Sua margem de economia está em {taxaPoupanca:F1}%. Tente reduzir gastos não essenciais."
                });
            }

            // Categoria Vilã
            if (!string.IsNullOrEmpty(analiseCategoria.CategoriaVila) && analiseCategoria.VilaVariacao > 20)
            {
                diagnosticos.Add(new DiagnosticoItem
                {
                    Icone = "trending_up",
                    Tipo = "danger",
                    Titulo = "Categoria em Alta",
                    Mensagem = $"'{analiseCategoria.CategoriaVila}' aumentou {analiseCategoria.VilaVariacao:F0}% comparado ao período anterior."
                });
            }

            // Categoria Heroína
            if (!string.IsNullOrEmpty(analiseCategoria.CategoriaHeroina) && analiseCategoria.HeroinaVariacao < -10)
            {
                diagnosticos.Add(new DiagnosticoItem
                {
                    Icone = "trending_down",
                    Tipo = "success",
                    Titulo = "Economia Identificada",
                    Mensagem = $"Ótimo! Você reduziu {Math.Abs(analiseCategoria.HeroinaVariacao):F0}% em '{analiseCategoria.CategoriaHeroina}'."
                });
            }

            // Maior Gasto
            if (gastosPorCategoria.Any())
            {
                var maior = gastosPorCategoria.First();
                var percentual = totalDespesas > 0 ? (maior.Total / totalDespesas) * 100 : 0;
                if (percentual > 40)
                {
                    diagnosticos.Add(new DiagnosticoItem
                    {
                        Icone = "pie_chart",
                        Tipo = "info",
                        Titulo = "Concentração de Gastos",
                        Mensagem = $"'{maior.Categoria}' representa {percentual:F0}% do total de despesas."
                    });
                }
            }

            // Dia da Semana com Maior Gasto
            if (gastosPorDia.Any())
            {
                var diaMaiorGasto = gastosPorDia.OrderByDescending(x => x.Total).First();
                if (diaMaiorGasto.Total > 0)
                {
                    diagnosticos.Add(new DiagnosticoItem
                    {
                        Icone = "calendar_today",
                        Tipo = "info",
                        Titulo = "Padrão Semanal",
                        Mensagem = $"Você gasta mais às {diaMaiorGasto.Dia}s. Atenção redobrada nesse dia!"
                    });
                }
            }

            return diagnosticos;
        }

        private static int ObterOrdemDiaSemana(string dia)
        {
            return dia switch
            {
                "Segunda" => 1,
                "Terça" => 2,
                "Quarta" => 3,
                "Quinta" => 4,
                "Sexta" => 5,
                "Sábado" => 6,
                "Domingo" => 7,
                _ => 8
            };
        }

        private static string EscapeCsv(string? valor)
        {
            if (string.IsNullOrEmpty(valor)) return "";
            if (valor.Contains(';') || valor.Contains('"') || valor.Contains('\n'))
            {
                return $"\"{valor.Replace("\"", "\"\"")}\"";
            }
            return valor;
        }

        #endregion
    }

    #region DTOs para Queries Otimizadas

    public class ReceitaDTO
    {
        public string? Titulo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool IsRecurring { get; set; }
        public string Recurrence { get; set; } = "Mensal";
        public int? RecurrenceCount { get; set; }
    }

    public class DespesaDTO
    {
        public string? Titulo { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public bool IsRecurring { get; set; }
        public string Recurrence { get; set; } = "Mensal";
        public int? RecurrenceCount { get; set; }
        public string CategoriaNome { get; set; } = "Sem Categoria";
    }

    #endregion
}