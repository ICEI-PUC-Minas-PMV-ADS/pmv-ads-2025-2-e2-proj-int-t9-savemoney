using System.ComponentModel.DataAnnotations;

namespace savemoney.Models.ViewModels
{
    /// <summary>
    /// ViewModel para a tela de KPIs Corporativos.
    /// Indicadores financeiros para visao PJ.
    /// </summary>
    public class KpisCorporativosViewModel
    {
        // ============================================
        // FILTROS
        // ============================================

        [Display(Name = "Data Inicio")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        // ============================================
        // KPI 1: MARGEM DE LUCRO
        // ============================================

        /// <summary>
        /// Total de receitas no periodo.
        /// </summary>
        public decimal ReceitaTotal { get; set; }

        /// <summary>
        /// Total de despesas no periodo.
        /// </summary>
        public decimal DespesaTotal { get; set; }

        /// <summary>
        /// Lucro = Receita - Despesa.
        /// </summary>
        public decimal LucroBruto => ReceitaTotal - DespesaTotal;

        /// <summary>
        /// Margem de Lucro = (Lucro / Receita) * 100.
        /// </summary>
        public decimal MargemLucro => ReceitaTotal > 0
            ? Math.Round((LucroBruto / ReceitaTotal) * 100, 2)
            : 0;

        /// <summary>
        /// Indica se teve lucro (true) ou prejuizo (false).
        /// </summary>
        public bool TemLucro => LucroBruto >= 0;

        // ============================================
        // KPI 2: BURN RATE
        // ============================================

        /// <summary>
        /// Total de custos fixos mensais.
        /// </summary>
        public decimal CustosFixosMensal { get; set; }

        /// <summary>
        /// Total de despesas operacionais mensais.
        /// </summary>
        public decimal DespesasOperacionaisMensal { get; set; }

        /// <summary>
        /// Burn Rate = Custos Fixos + Despesas Operacionais.
        /// Quanto a empresa "queima" por mes.
        /// </summary>
        public decimal BurnRate => CustosFixosMensal + DespesasOperacionaisMensal;

        /// <summary>
        /// Saldo disponivel atual (caixa).
        /// </summary>
        public decimal SaldoDisponivel { get; set; }

        /// <summary>
        /// Runway = Saldo / Burn Rate.
        /// Quantos meses a empresa sobrevive com o caixa atual.
        /// </summary>
        public decimal RunwayMeses => BurnRate > 0
            ? Math.Round(SaldoDisponivel / BurnRate, 1)
            : 0;

        // ============================================
        // KPI 3: PONTO DE EQUILIBRIO
        // ============================================

        /// <summary>
        /// Custos variaveis totais.
        /// </summary>
        public decimal CustosVariaveis { get; set; }

        /// <summary>
        /// Margem de contribuicao percentual.
        /// MC% = (Receita - Custos Variaveis) / Receita * 100
        /// </summary>
        public decimal MargemContribuicaoPercentual => ReceitaTotal > 0
            ? Math.Round(((ReceitaTotal - CustosVariaveis) / ReceitaTotal) * 100, 2)
            : 0;

        /// <summary>
        /// Ponto de Equilibrio = Custos Fixos / MC%
        /// Receita necessaria para cobrir todos os custos.
        /// </summary>
        public decimal PontoEquilibrio => MargemContribuicaoPercentual > 0
            ? Math.Round((CustosFixosMensal + DespesasOperacionaisMensal) / (MargemContribuicaoPercentual / 100), 2)
            : 0;

        /// <summary>
        /// Percentual atingido do ponto de equilibrio.
        /// </summary>
        public decimal PercentualAtingido => PontoEquilibrio > 0
            ? Math.Round((ReceitaTotal / PontoEquilibrio) * 100, 1)
            : 0;

        /// <summary>
        /// Indica se atingiu o ponto de equilibrio.
        /// </summary>
        public bool AtingiuEquilibrio => ReceitaTotal >= PontoEquilibrio && PontoEquilibrio > 0;

        // ============================================
        // DADOS PARA GRAFICOS
        // ============================================

        /// <summary>
        /// Historico mensal para grafico de evolucao.
        /// </summary>
        public List<KpiMensal> HistoricoMensal { get; set; } = new();

        /// <summary>
        /// Detalhamento de custos por categoria.
        /// </summary>
        public List<CustoCategoria> CustosPorCategoria { get; set; } = new();
    }

    /// <summary>
    /// Dados mensais para grafico.
    /// </summary>
    public class KpiMensal
    {
        public int Ano { get; set; }
        public int Mes { get; set; }

        public string MesAnoAbreviado
        {
            get
            {
                var meses = new[] { "", "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
                                        "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
                return $"{meses[Mes]}/{Ano.ToString().Substring(2)}";
            }
        }

        public decimal Receita { get; set; }
        public decimal Despesa { get; set; }
        public decimal Lucro => Receita - Despesa;
        public decimal MargemLucro => Receita > 0 ? Math.Round((Lucro / Receita) * 100, 2) : 0;
    }

    /// <summary>
    /// Custo agrupado por categoria.
    /// </summary>
    public class CustoCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Percentual { get; set; }
        public string Cor { get; set; } = "#3b82f6";
    }
}