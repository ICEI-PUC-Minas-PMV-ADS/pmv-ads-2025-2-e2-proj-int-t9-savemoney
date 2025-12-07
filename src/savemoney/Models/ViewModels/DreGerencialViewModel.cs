using System.ComponentModel.DataAnnotations;
using savemoney.Models.Enums;

namespace savemoney.Models.ViewModels
{
    /// <summary>
    /// ViewModel principal para a tela de DRE Gerencial Simplificado.
    /// Contém filtros, dados calculados e histórico para gráficos.
    /// </summary>
    public class DreGerencialViewModel
    {
        // ============================================
        // FILTROS
        // ============================================

        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        [Display(Name = "Regime de Apuração")]
        public TipoRegimeDre Regime { get; set; } = TipoRegimeDre.Competencia;

        // ============================================
        // PERÍODO COMPARATIVO (OPCIONAL)
        // ============================================

        [Display(Name = "Habilitar Comparativo")]
        public bool HabilitarComparativo { get; set; } = false;

        [Display(Name = "Início Comparativo")]
        [DataType(DataType.Date)]
        public DateTime? DataInicioComparativo { get; set; }

        [Display(Name = "Fim Comparativo")]
        [DataType(DataType.Date)]
        public DateTime? DataFimComparativo { get; set; }

        // ============================================
        // DADOS DO DRE
        // ============================================

        /// <summary>
        /// Dados do período principal selecionado.
        /// </summary>
        public DrePeriodo PeriodoAtual { get; set; } = new();

        /// <summary>
        /// Dados do período comparativo (quando habilitado).
        /// </summary>
        public DrePeriodo? PeriodoComparativo { get; set; }

        /// <summary>
        /// Variação percentual entre períodos (quando comparativo habilitado).
        /// </summary>
        public DreVariacao? Variacao { get; set; }

        // ============================================
        // HISTÓRICO MENSAL (PARA GRÁFICOS)
        // ============================================

        /// <summary>
        /// Lista de dados mensais para gráficos de evolução.
        /// </summary>
        public List<DreMensal> HistoricoMensal { get; set; } = new();
    }

    /// <summary>
    /// Representa os dados de um período específico do DRE.
    /// Estrutura: Receita → Custos → Margem → Despesas → Lucro
    /// </summary>
    public class DrePeriodo
    {
        /// <summary>
        /// Descrição do período (ex: "Janeiro/2025" ou "01/01/2025 a 31/01/2025").
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        // ============================================
        // (+) RECEITAS
        // ============================================

        /// <summary>
        /// Total de receitas brutas no período.
        /// </summary>
        public decimal ReceitaBruta { get; set; }

        /// <summary>
        /// Detalhamento das receitas por categoria.
        /// </summary>
        public List<DreLinhaDetalhe> ReceitasDetalhadas { get; set; } = new();

        // ============================================
        // (-) CUSTOS VARIÁVEIS
        // ============================================

        /// <summary>
        /// Total de custos variáveis (CMV, custos diretos de produção/serviço).
        /// </summary>
        public decimal CustosVariaveis { get; set; }

        /// <summary>
        /// Detalhamento dos custos por categoria.
        /// </summary>
        public List<DreLinhaDetalhe> CustosDetalhados { get; set; } = new();

        // ============================================
        // (=) MARGEM DE CONTRIBUIÇÃO
        // ============================================

        /// <summary>
        /// Margem de Contribuição = Receita Bruta - Custos Variáveis.
        /// Indica quanto sobra para cobrir despesas fixas e gerar lucro.
        /// </summary>
        public decimal MargemContribuicao => ReceitaBruta - CustosVariaveis;

        /// <summary>
        /// Margem de Contribuição em percentual sobre a receita.
        /// </summary>
        public decimal MargemContribuicaoPercentual =>
            ReceitaBruta > 0 ? Math.Round((MargemContribuicao / ReceitaBruta) * 100, 2) : 0;

        // ============================================
        // (-) DESPESAS OPERACIONAIS/FIXAS
        // ============================================

        /// <summary>
        /// Total de despesas fixas/operacionais (aluguel, salários, administrativo).
        /// </summary>
        public decimal DespesasOperacionais { get; set; }

        /// <summary>
        /// Detalhamento das despesas por categoria.
        /// </summary>
        public List<DreLinhaDetalhe> DespesasDetalhadas { get; set; } = new();

        // ============================================
        // (=) RESULTADO (LUCRO/PREJUÍZO)
        // ============================================

        /// <summary>
        /// Lucro Líquido = Margem de Contribuição - Despesas Operacionais.
        /// Valor negativo indica prejuízo.
        /// </summary>
        public decimal LucroLiquido => MargemContribuicao - DespesasOperacionais;

        /// <summary>
        /// Margem Líquida em percentual sobre a receita.
        /// </summary>
        public decimal MargemLiquidaPercentual =>
            ReceitaBruta > 0 ? Math.Round((LucroLiquido / ReceitaBruta) * 100, 2) : 0;

        /// <summary>
        /// Indica se o período teve lucro (true) ou prejuízo (false).
        /// </summary>
        public bool TemLucro => LucroLiquido >= 0;
    }

    /// <summary>
    /// Linha de detalhamento por categoria no DRE.
    /// </summary>
    public class DreLinhaDetalhe
    {
        /// <summary>
        /// Nome da categoria.
        /// </summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Valor total da categoria no período.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Percentual em relação ao total do grupo.
        /// </summary>
        public decimal Percentual { get; set; }

        /// <summary>
        /// Quantidade de lançamentos na categoria.
        /// </summary>
        public int QuantidadeLancamentos { get; set; }
    }

    /// <summary>
    /// Dados mensais para gráfico de evolução do DRE.
    /// </summary>
    public class DreMensal
    {
        public int Ano { get; set; }
        public int Mes { get; set; }

        /// <summary>
        /// Formato para exibição: "Jan/25", "Fev/25", etc.
        /// </summary>
        public string MesAnoAbreviado
        {
            get
            {
                var meses = new[] { "", "Jan", "Fev", "Mar", "Abr", "Mai", "Jun",
                                        "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
                return $"{meses[Mes]}/{Ano.ToString().Substring(2)}";
            }
        }

        /// <summary>
        /// Formato completo: "01/2025".
        /// </summary>
        public string MesAnoCompleto => $"{Mes:D2}/{Ano}";

        public decimal ReceitaBruta { get; set; }
        public decimal CustosVariaveis { get; set; }
        public decimal MargemContribuicao => ReceitaBruta - CustosVariaveis;
        public decimal DespesasOperacionais { get; set; }
        public decimal LucroLiquido => MargemContribuicao - DespesasOperacionais;
    }

    /// <summary>
    /// Variação percentual entre dois períodos comparados.
    /// </summary>
    public class DreVariacao
    {
        public decimal ReceitaBrutaVariacao { get; set; }
        public decimal CustosVariaveisVariacao { get; set; }
        public decimal MargemContribuicaoVariacao { get; set; }
        public decimal DespesasOperacionaisVariacao { get; set; }
        public decimal LucroLiquidoVariacao { get; set; }

        /// <summary>
        /// Calcula variação percentual entre dois valores.
        /// </summary>
        public static decimal CalcularVariacao(decimal valorAtual, decimal valorAnterior)
        {
            if (valorAnterior == 0)
                return valorAtual > 0 ? 100 : (valorAtual < 0 ? -100 : 0);

            return Math.Round(((valorAtual - valorAnterior) / Math.Abs(valorAnterior)) * 100, 2);
        }
    }
}