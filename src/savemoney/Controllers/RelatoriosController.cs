using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Models.ViewModels;
using savemoney.Services; // Namespace onde está o ServicoCalculoRecorrencia
using System.Security.Claims;
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
        public async Task<IActionResult> Index(DateTime? dataInicio, DateTime? dataFim)
        {
            // 1. Identificar Usuário Logado
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return RedirectToAction("Login", "Usuarios");
            var userId = int.Parse(userIdClaim);

            // 2. Definir Período (Padrão: Início do mês atual até hoje)
            var inicio = dataInicio ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fim = dataFim ?? DateTime.Now;

            // Garantir que inicio não seja maior que fim
            if (inicio > fim) inicio = fim.AddDays(-30);

            // 3. Buscar Dados Brutos (Sem filtros de data aqui, pois precisamos calcular recorrências antigas)
            // AsNoTracking melhora a performance pois é apenas leitura
            var receitasBrutas = await _context.Receitas
                .AsNoTracking()
                .Where(r => r.UsuarioId == userId)
                .ToListAsync();

            var despesasBrutas = await _context.Despesas
                .AsNoTracking()
                .Include(d => d.BudgetCategory)
                .ThenInclude(bc => bc!.Category) // <--- ADICIONADO "!" AQUI
                .Where(d => d.UsuarioId == userId)
                .ToListAsync();

            // 4. Processar Recorrências (Matemática Financeira via Serviço)
            var transacoesProcessadas = ServicoCalculoRecorrencia.ProcessarTransacoes(
                receitasBrutas,
                despesasBrutas,
                inicio,
                fim
            );

            // 5. Calcular KPIs (Indicadores)
            var totalReceitas = transacoesProcessadas.Where(t => t.EhReceita).Sum(t => t.Valor);
            var totalDespesas = transacoesProcessadas.Where(t => !t.EhReceita).Sum(t => t.Valor);
            var saldo = totalReceitas - totalDespesas;

            // Taxa de Poupança (evitar divisão por zero)
            var taxaPoupanca = totalReceitas > 0 ? (saldo / totalReceitas) * 100 : 0;

            // 6. Preparar Dados para Gráfico de Fluxo (Linha do Tempo)
            var labelsDatas = new List<string>();
            var valoresReceitas = new List<decimal>();
            var valoresDespesas = new List<decimal>();

            // Agrupar por dia para somar transações do mesmo dia
            var fluxoPorDia = transacoesProcessadas
                .GroupBy(t => t.Data.Date)
                .ToDictionary(g => g.Key, g => new {
                    Receita = g.Where(x => x.EhReceita).Sum(x => x.Valor),
                    Despesa = g.Where(x => !x.EhReceita).Sum(x => x.Valor)
                });

            // Preencher todos os dias do período (mesmo os vazios)
            for (var data = inicio.Date; data <= fim.Date; data = data.AddDays(1))
            {
                labelsDatas.Add(data.ToString("dd/MM"));
                if (fluxoPorDia.ContainsKey(data))
                {
                    valoresReceitas.Add(fluxoPorDia[data].Receita);
                    valoresDespesas.Add(fluxoPorDia[data].Despesa);
                }
                else
                {
                    valoresReceitas.Add(0);
                    valoresDespesas.Add(0);
                }
            }

            // 7. Preparar Dados para Gráfico de Categorias (Top Gastos)
            var gastosPorCategoria = transacoesProcessadas
                .Where(t => !t.EhReceita) // Apenas despesas
                .GroupBy(t => t.Categoria)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Valor) })
                .OrderByDescending(x => x.Total)
                .ToList();

            // 8. Gerar Diagnósticos Simples
            var diagnosticos = new List<string>();
            if (totalDespesas > totalReceitas)
                diagnosticos.Add("⚠️ Alerta: Você está gastando mais do que ganha neste período.");

            if (taxaPoupanca >= 20)
                diagnosticos.Add("✅ Excelente: Você está poupando 20% ou mais da sua renda!");
            else if (taxaPoupanca > 0 && taxaPoupanca < 10)
                diagnosticos.Add("ℹ️ Atenção: Sua margem financeira está baixa (menos de 10%).");

            if (gastosPorCategoria.Any())
            {
                var maiorGasto = gastosPorCategoria.First();
                var pctMaiorGasto = totalDespesas > 0 ? (maiorGasto.Total / totalDespesas) * 100 : 0;
                if (pctMaiorGasto > 40)
                    diagnosticos.Add($"📊 Dica: '{maiorGasto.Categoria}' consome {pctMaiorGasto:F0}% do seu orçamento. Tente reduzir.");
            }

            // 9. Montar ViewModel
            var viewModel = new RelatorioFinanceiroViewModel
            {
                DataInicio = inicio,
                DataFim = fim,
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = saldo,
                TaxaPoupanca = taxaPoupanca,
                TransacoesDetalhadas = transacoesProcessadas, // Lista completa para tabela
                Diagnosticos = diagnosticos,

                // Serializando para JSON para o JavaScript ler
                JsonLabelsTimeline = JsonSerializer.Serialize(labelsDatas),
                JsonDadosReceitas = JsonSerializer.Serialize(valoresReceitas),
                JsonDadosDespesas = JsonSerializer.Serialize(valoresDespesas),
                JsonLabelsCategorias = JsonSerializer.Serialize(gastosPorCategoria.Select(x => x.Categoria)),
                JsonValoresCategorias = JsonSerializer.Serialize(gastosPorCategoria.Select(x => x.Total))
            };

            return View(viewModel);
        }
    }
}