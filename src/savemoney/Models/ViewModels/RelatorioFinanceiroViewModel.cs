using System;
using System.Collections.Generic;
using System.Globalization;

namespace savemoney.Models.ViewModels
{
    /// <summary>
    /// ViewModel completo para a tela de relatórios financeiros
    /// </summary>
    public class RelatorioFinanceiroViewModel
    {
        #region Filtros Aplicados

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public PeriodoAgregacao Agregacao { get; set; } = PeriodoAgregacao.Mensal;
        public string? TipoRecorrenciaFiltro { get; set; }
        public string? CategoriaFiltro { get; set; }
        public List<string> CategoriasDisponiveis { get; set; } = new();

        #endregion

        #region KPIs do Período Atual

        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo { get; set; }
        public decimal TaxaPoupanca { get; set; }

        /// <summary>
        /// Classe CSS para o saldo (text-success ou text-danger)
        /// </summary>
        public string ClasseCssSaldo => Saldo >= 0 ? "text-success" : "text-danger";

        /// <summary>
        /// Ícone para o saldo
        /// </summary>
        public string IconeSaldo => Saldo >= 0 ? "trending_up" : "trending_down";

        #endregion

        #region KPIs do Período Anterior (Comparação)

        public decimal ReceitasAnterior { get; set; }
        public decimal DespesasAnterior { get; set; }
        public decimal SaldoAnterior { get; set; }

        /// <summary>
        /// Variação percentual das receitas em relação ao período anterior
        /// </summary>
        public decimal VariacaoReceitas { get; set; }

        /// <summary>
        /// Variação percentual das despesas em relação ao período anterior
        /// </summary>
        public decimal VariacaoDespesas { get; set; }

        /// <summary>
        /// Variação percentual do saldo em relação ao período anterior
        /// </summary>
        public decimal VariacaoSaldo { get; set; }

        /// <summary>
        /// Classe CSS para a variação de receitas
        /// </summary>
        public string ClasseCssVariacaoReceitas => VariacaoReceitas >= 0 ? "text-success" : "text-danger";

        /// <summary>
        /// Classe CSS para a variação de despesas (invertido: menos despesa = bom)
        /// </summary>
        public string ClasseCssVariacaoDespesas => VariacaoDespesas <= 0 ? "text-success" : "text-danger";

        /// <summary>
        /// Classe CSS para a variação do saldo
        /// </summary>
        public string ClasseCssVariacaoSaldo => VariacaoSaldo >= 0 ? "text-success" : "text-danger";

        /// <summary>
        /// Ícone para variação de receitas
        /// </summary>
        public string IconeVariacaoReceitas => VariacaoReceitas >= 0 ? "arrow_upward" : "arrow_downward";

        /// <summary>
        /// Ícone para variação de despesas
        /// </summary>
        public string IconeVariacaoDespesas => VariacaoDespesas >= 0 ? "arrow_upward" : "arrow_downward";

        /// <summary>
        /// Ícone para variação do saldo
        /// </summary>
        public string IconeVariacaoSaldo => VariacaoSaldo >= 0 ? "arrow_upward" : "arrow_downward";

        #endregion

        #region Análise de Categorias

        /// <summary>
        /// Categoria com maior aumento de gastos
        /// </summary>
        public string? CategoriaVila { get; set; }

        /// <summary>
        /// Variação percentual da categoria vilã
        /// </summary>
        public decimal CategoriaVilaVariacao { get; set; }

        /// <summary>
        /// Categoria com maior redução de gastos
        /// </summary>
        public string? CategoriaHeroina { get; set; }

        /// <summary>
        /// Variação percentual da categoria heroína
        /// </summary>
        public decimal CategoriaHeroinaVariacao { get; set; }

        #endregion

        #region Projeção Futura

        public List<string> ProjecaoMeses { get; set; } = new();
        public List<decimal> ProjecaoSaldos { get; set; } = new();
        public string JsonProjecaoMeses { get; set; } = "[]";
        public string JsonProjecaoSaldos { get; set; } = "[]";

        #endregion

        #region Listas de Dados

        /// <summary>
        /// Lista detalhada de todas as transações processadas
        /// </summary>
        public List<TransacaoProcessada> TransacoesDetalhadas { get; set; } = new();

        /// <summary>
        /// Top 5 maiores receitas
        /// </summary>
        public List<TopItemViewModel> TopReceitas { get; set; } = new();

        /// <summary>
        /// Top 5 maiores despesas
        /// </summary>
        public List<TopItemViewModel> TopDespesas { get; set; } = new();

        /// <summary>
        /// Lista de diagnósticos automáticos
        /// </summary>
        public List<DiagnosticoItem> Diagnosticos { get; set; } = new();

        #endregion

        #region Dados JSON para Gráficos

        /// <summary>
        /// Labels do eixo X do gráfico de timeline
        /// </summary>
        public string JsonLabelsTimeline { get; set; } = "[]";

        /// <summary>
        /// Valores de receitas para o gráfico
        /// </summary>
        public string JsonDadosReceitas { get; set; } = "[]";

        /// <summary>
        /// Valores de despesas para o gráfico
        /// </summary>
        public string JsonDadosDespesas { get; set; } = "[]";

        /// <summary>
        /// Labels das categorias para gráfico de rosca
        /// </summary>
        public string JsonLabelsCategorias { get; set; } = "[]";

        /// <summary>
        /// Valores das categorias para gráfico de rosca
        /// </summary>
        public string JsonValoresCategorias { get; set; } = "[]";

        /// <summary>
        /// Dias da semana para gráfico de análise
        /// </summary>
        public string JsonDiasSemana { get; set; } = "[]";

        /// <summary>
        /// Valores por dia da semana
        /// </summary>
        public string JsonValoresDiaSemana { get; set; } = "[]";

        #endregion

        #region Controle de Estado

        /// <summary>
        /// Indica se há dados suficientes para exibir os relatórios
        /// </summary>
        public bool TemDadosSuficientes { get; set; }

        /// <summary>
        /// Quantidade total de transações no período
        /// </summary>
        public int QuantidadeTransacoes { get; set; }

        /// <summary>
        /// Texto amigável para período vazio
        /// </summary>
        public string MensagemVazia => "Nenhuma transação encontrada para o período selecionado. Adicione receitas ou despesas para visualizar seu relatório.";

        #endregion
    }

    /// <summary>
    /// Item para listas de Top receitas/despesas
    /// </summary>
    public class TopItemViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public string? Categoria { get; set; }

        /// <summary>
        /// Valor formatado em moeda
        /// </summary>
        public string ValorFormatado => Valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
    }

    /// <summary>
    /// Item de diagnóstico automático
    /// </summary>
    public class DiagnosticoItem
    {
        public string Icone { get; set; } = "info";
        public string Tipo { get; set; } = "info"; // success, warning, danger, info
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Classe CSS baseada no tipo
        /// </summary>
        public string ClasseCss => Tipo switch
        {
            "success" => "diag-success",
            "warning" => "diag-warning",
            "danger" => "diag-danger",
            _ => "diag-info"
        };

        /// <summary>
        /// Cor do ícone baseada no tipo
        /// </summary>
        public string CorIcone => Tipo switch
        {
            "success" => "var(--success)",
            "warning" => "var(--warning)",
            "danger" => "var(--danger)",
            _ => "var(--accent-primary)"
        };
    }
}