using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Models.Enums;
using savemoney.Models.ViewModels;
using savemoney.services.Interfaces;

namespace savemoney.Services
{
    /// <summary>
    /// Serviço responsável pelos cálculos do DRE Gerencial Simplificado.
    /// </summary>
    public class DreGerencialService : IDreGerencialService
    {
        private readonly AppDbContext _context;

        public DreGerencialService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<DreGerencialViewModel> GerarDreAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim,
            TipoRegimeDre regime)
        {
            var viewModel = new DreGerencialViewModel
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                Regime = regime,
                PeriodoAtual = await CalcularPeriodoAsync(usuarioId, dataInicio, dataFim, regime)
            };

            viewModel.PeriodoAtual.Descricao = FormatarDescricaoPeriodo(dataInicio, dataFim);

            return viewModel;
        }

        /// <inheritdoc />
        public async Task<DreGerencialViewModel> GerarDreComparativoAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim,
            DateTime dataInicioComparativo,
            DateTime dataFimComparativo,
            TipoRegimeDre regime)
        {
            var viewModel = await GerarDreAsync(usuarioId, dataInicio, dataFim, regime);

            viewModel.HabilitarComparativo = true;
            viewModel.DataInicioComparativo = dataInicioComparativo;
            viewModel.DataFimComparativo = dataFimComparativo;

            viewModel.PeriodoComparativo = await CalcularPeriodoAsync(
                usuarioId, dataInicioComparativo, dataFimComparativo, regime);
            viewModel.PeriodoComparativo.Descricao = FormatarDescricaoPeriodo(
                dataInicioComparativo, dataFimComparativo);

            // Calcula variações
            viewModel.Variacao = CalcularVariacao(viewModel.PeriodoAtual, viewModel.PeriodoComparativo);

            return viewModel;
        }

        /// <inheritdoc />
        public async Task<List<DreMensal>> ObterHistoricoMensalAsync(
            int usuarioId,
            int meses = 12,
            TipoRegimeDre regime = TipoRegimeDre.Competencia)
        {
            var historico = new List<DreMensal>();
            var dataAtual = DateTime.Today;

            for (int i = meses - 1; i >= 0; i--)
            {
                var dataRef = dataAtual.AddMonths(-i);
                var inicioMes = new DateTime(dataRef.Year, dataRef.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                var periodo = await CalcularPeriodoAsync(usuarioId, inicioMes, fimMes, regime);

                historico.Add(new DreMensal
                {
                    Ano = dataRef.Year,
                    Mes = dataRef.Month,
                    ReceitaBruta = periodo.ReceitaBruta,
                    CustosVariaveis = periodo.CustosVariaveis,
                    DespesasOperacionais = periodo.DespesasOperacionais
                });
            }

            return historico;
        }

        /// <summary>
        /// Calcula os dados do DRE para um período específico.
        /// </summary>
        private async Task<DrePeriodo> CalcularPeriodoAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim,
            TipoRegimeDre regime)
        {
            var periodo = new DrePeriodo();

            // ============================================
            // RECEITAS
            // ============================================
            var queryReceitas = _context.Receitas
                .Include(r => r.Category)
                .Where(r => r.UsuarioId == usuarioId);

            // Aplica filtro por regime
            if (regime == TipoRegimeDre.Competencia)
            {
                // Competência: data do fato gerador
                queryReceitas = queryReceitas.Where(r =>
                    r.DataInicio >= dataInicio && r.DataInicio <= dataFim);
            }
            else
            {
                // Caixa: somente recebidas no período
                queryReceitas = queryReceitas.Where(r =>
                    r.Recebido &&
                    r.DataInicio >= dataInicio && r.DataInicio <= dataFim);
            }

            var receitas = await queryReceitas.ToListAsync();
            periodo.ReceitaBruta = receitas.Sum(r => r.Valor);

            // Agrupa receitas por categoria
            periodo.ReceitasDetalhadas = receitas
                .GroupBy(r => r.Category?.Name ?? "Receitas Gerais")
                .Select(g => new DreLinhaDetalhe
                {
                    Categoria = g.Key,
                    Valor = g.Sum(r => r.Valor),
                    Percentual = periodo.ReceitaBruta > 0
                        ? Math.Round((g.Sum(r => r.Valor) / periodo.ReceitaBruta) * 100, 2)
                        : 0,
                    QuantidadeLancamentos = g.Count()
                })
                .OrderByDescending(x => x.Valor)
                .ToList();

            // ============================================
            // DESPESAS (CUSTOS VARIÁVEIS + OPERACIONAIS)
            // ============================================
            var queryDespesas = _context.Despesas
                .Include(d => d.Category)
                .Include(d => d.BudgetCategory)
                    .ThenInclude(bc => bc!.Category)
                .Where(d => d.UsuarioId == usuarioId);

            // Aplica filtro por regime
            if (regime == TipoRegimeDre.Competencia)
            {
                queryDespesas = queryDespesas.Where(d =>
                    d.DataInicio >= dataInicio && d.DataInicio <= dataFim);
            }
            else
            {
                // Caixa: somente pagas no período
                queryDespesas = queryDespesas.Where(d =>
                    d.Pago &&
                    d.DataInicio >= dataInicio && d.DataInicio <= dataFim);
            }

            var despesas = await queryDespesas.ToListAsync();

            // Separa por classificação DRE (prioriza Category direto, fallback para BudgetCategory)
            var custosVariaveis = despesas
                .Where(d => ObterClassificacao(d) == TipoClassificacaoDre.CustoVariavel)
                .ToList();

            var despesasOperacionais = despesas
                .Where(d => ObterClassificacao(d) == TipoClassificacaoDre.DespesaOperacional)
                .ToList();

            // Despesas não classificadas vão para Operacionais
            var naoClassificadas = despesas
                .Where(d => ObterClassificacao(d) == TipoClassificacaoDre.NaoClassificado)
                .ToList();

            despesasOperacionais.AddRange(naoClassificadas);

            // ============================================
            // CUSTOS VARIÁVEIS
            // ============================================
            periodo.CustosVariaveis = custosVariaveis.Sum(d => d.Valor);

            periodo.CustosDetalhados = custosVariaveis
                .GroupBy(d => ObterNomeCategoria(d))
                .Select(g => new DreLinhaDetalhe
                {
                    Categoria = g.Key,
                    Valor = g.Sum(d => d.Valor),
                    Percentual = periodo.CustosVariaveis > 0
                        ? Math.Round((g.Sum(d => d.Valor) / periodo.CustosVariaveis) * 100, 2)
                        : 0,
                    QuantidadeLancamentos = g.Count()
                })
                .OrderByDescending(x => x.Valor)
                .ToList();

            // ============================================
            // DESPESAS OPERACIONAIS
            // ============================================
            periodo.DespesasOperacionais = despesasOperacionais.Sum(d => d.Valor);

            periodo.DespesasDetalhadas = despesasOperacionais
                .GroupBy(d => ObterNomeCategoria(d))
                .Select(g => new DreLinhaDetalhe
                {
                    Categoria = g.Key,
                    Valor = g.Sum(d => d.Valor),
                    Percentual = periodo.DespesasOperacionais > 0
                        ? Math.Round((g.Sum(d => d.Valor) / periodo.DespesasOperacionais) * 100, 2)
                        : 0,
                    QuantidadeLancamentos = g.Count()
                })
                .OrderByDescending(x => x.Valor)
                .ToList();

            return periodo;
        }

        /// <summary>
        /// Calcula a variação percentual entre dois períodos.
        /// </summary>
        private DreVariacao CalcularVariacao(DrePeriodo atual, DrePeriodo anterior)
        {
            return new DreVariacao
            {
                ReceitaBrutaVariacao = DreVariacao.CalcularVariacao(atual.ReceitaBruta, anterior.ReceitaBruta),
                CustosVariaveisVariacao = DreVariacao.CalcularVariacao(atual.CustosVariaveis, anterior.CustosVariaveis),
                MargemContribuicaoVariacao = DreVariacao.CalcularVariacao(atual.MargemContribuicao, anterior.MargemContribuicao),
                DespesasOperacionaisVariacao = DreVariacao.CalcularVariacao(atual.DespesasOperacionais, anterior.DespesasOperacionais),
                LucroLiquidoVariacao = DreVariacao.CalcularVariacao(atual.LucroLiquido, anterior.LucroLiquido)
            };
        }

        /// <summary>
        /// Formata a descrição do período para exibição.
        /// </summary>
        private string FormatarDescricaoPeriodo(DateTime inicio, DateTime fim)
        {
            // Se for mês completo
            if (inicio.Day == 1 && fim.Day == DateTime.DaysInMonth(fim.Year, fim.Month) &&
                inicio.Month == fim.Month && inicio.Year == fim.Year)
            {
                var meses = new[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                                    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };
                return $"{meses[inicio.Month]}/{inicio.Year}";
            }

            return $"{inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}";
        }

        /// <summary>
        /// Obtém a classificação DRE da despesa.
        /// Prioriza Category direto, fallback para BudgetCategory.
        /// </summary>
        private TipoClassificacaoDre ObterClassificacao(Despesa d)
        {
            // Prioridade 1: Category direta
            if (d.Category != null)
                return d.Category.ClassificacaoDre;

            // Prioridade 2: Category via BudgetCategory
            if (d.BudgetCategory?.Category != null)
                return d.BudgetCategory.Category.ClassificacaoDre;

            return TipoClassificacaoDre.NaoClassificado;
        }

        /// <summary>
        /// Obtém o nome da categoria da despesa.
        /// Prioriza Category direto, fallback para BudgetCategory.
        /// </summary>
        private string ObterNomeCategoria(Despesa d)
        {
            // Prioridade 1: Category direta
            if (d.Category != null)
                return d.Category.Name;

            // Prioridade 2: Category via BudgetCategory
            if (d.BudgetCategory?.Category != null)
                return d.BudgetCategory.Category.Name;

            return "Sem Categoria";
        }
    }
}