using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Models.Enums;
using savemoney.Models.ViewModels;
using savemoney.services.Interfaces;

namespace savemoney.Services
{
    /// <summary>
    /// Servico responsavel pelos calculos de KPIs Corporativos.
    /// </summary>
    public class KpisCorporativosService : IKpisCorporativosService
    {
        private readonly AppDbContext _context;

        // Cores para o grafico de categorias
        private static readonly string[] CoresCategorias = new[]
        {
            "#3b82f6", "#10b981", "#f59e0b", "#ef4444", "#8b5cf6",
            "#ec4899", "#06b6d4", "#84cc16", "#f97316", "#6366f1"
        };

        public KpisCorporativosService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<KpisCorporativosViewModel> CalcularKpisAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim)
        {
            var viewModel = new KpisCorporativosViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim
            };

            // ============================================
            // RECEITAS
            // ============================================
            var receitas = await _context.Receitas
                .Where(r => r.UsuarioId == usuarioId &&
                           r.DataInicio >= dataInicio &&
                           r.DataInicio <= dataFim)
                .ToListAsync();

            viewModel.ReceitaTotal = receitas.Sum(r => r.Valor);

            // ============================================
            // DESPESAS (com categoria para classificacao)
            // ============================================
            var despesas = await _context.Despesas
                .Include(d => d.Category)
                .Where(d => d.UsuarioId == usuarioId &&
                           d.DataInicio >= dataInicio &&
                           d.DataInicio <= dataFim)
                .ToListAsync();

            viewModel.DespesaTotal = despesas.Sum(d => d.Valor);

            // Separa por tipo contabil
            viewModel.CustosVariaveis = despesas
                .Where(d => d.Category?.TipoContabil == TipoContabil.CustoVariavel)
                .Sum(d => d.Valor);

            viewModel.CustosFixosMensal = despesas
                .Where(d => d.Category?.TipoContabil == TipoContabil.DespesaFixa)
                .Sum(d => d.Valor);

            viewModel.DespesasOperacionaisMensal = despesas
                .Where(d => d.Category?.TipoContabil == TipoContabil.DespesaOperacional ||
                           d.Category?.TipoContabil == TipoContabil.NaoClassificado ||
                           d.Category == null)
                .Sum(d => d.Valor);

            // ============================================
            // SALDO DISPONIVEL (para Runway)
            // ============================================
            var todasReceitas = await _context.Receitas
                .Where(r => r.UsuarioId == usuarioId && r.Recebido)
                .SumAsync(r => r.Valor);

            var todasDespesas = await _context.Despesas
                .Where(d => d.UsuarioId == usuarioId && d.Pago)
                .SumAsync(d => d.Valor);

            viewModel.SaldoDisponivel = todasReceitas - todasDespesas;

            // ============================================
            // HISTORICO E CATEGORIAS
            // ============================================
            viewModel.HistoricoMensal = await ObterHistoricoMensalAsync(usuarioId, 6);
            viewModel.CustosPorCategoria = await ObterCustosPorCategoriaAsync(usuarioId, dataInicio, dataFim);

            return viewModel;
        }

        /// <inheritdoc />
        public async Task<List<KpiMensal>> ObterHistoricoMensalAsync(
            int usuarioId,
            int meses = 6)
        {
            var historico = new List<KpiMensal>();
            var dataAtual = DateTime.Today;

            for (int i = meses - 1; i >= 0; i--)
            {
                var dataRef = dataAtual.AddMonths(-i);
                var inicioMes = new DateTime(dataRef.Year, dataRef.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                var receitaMes = await _context.Receitas
                    .Where(r => r.UsuarioId == usuarioId &&
                               r.DataInicio >= inicioMes &&
                               r.DataInicio <= fimMes)
                    .SumAsync(r => r.Valor);

                var despesaMes = await _context.Despesas
                    .Where(d => d.UsuarioId == usuarioId &&
                               d.DataInicio >= inicioMes &&
                               d.DataInicio <= fimMes)
                    .SumAsync(d => d.Valor);

                historico.Add(new KpiMensal
                {
                    Ano = dataRef.Year,
                    Mes = dataRef.Month,
                    Receita = receitaMes,
                    Despesa = despesaMes
                });
            }

            return historico;
        }

        /// <inheritdoc />
        public async Task<List<CustoCategoria>> ObterCustosPorCategoriaAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim)
        {
            var despesas = await _context.Despesas
                .Include(d => d.Category)
                .Where(d => d.UsuarioId == usuarioId &&
                           d.DataInicio >= dataInicio &&
                           d.DataInicio <= dataFim)
                .ToListAsync();

            var totalDespesas = despesas.Sum(d => d.Valor);

            var custosPorCategoria = despesas
                .GroupBy(d => d.Category?.Name ?? "Sem Categoria")
                .Select((g, index) => new CustoCategoria
                {
                    Categoria = g.Key,
                    Valor = g.Sum(d => d.Valor),
                    Percentual = totalDespesas > 0
                        ? Math.Round((g.Sum(d => d.Valor) / totalDespesas) * 100, 1)
                        : 0,
                    Cor = CoresCategorias[index % CoresCategorias.Length]
                })
                .OrderByDescending(c => c.Valor)
                .Take(10)
                .ToList();

            return custosPorCategoria;
        }
    }
}