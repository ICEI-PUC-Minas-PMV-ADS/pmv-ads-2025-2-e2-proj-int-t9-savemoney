using System;
using System.Collections.Generic;

namespace savemoney.Models
{
    /// <summary>
    /// ViewModel principal que encapsula todo o resultado da análise de tendências
    /// Este objeto é retornado pelo Service e consumido pela View
    /// </summary>
    public class RelatorioTendenciaViewModel
    {
        /// <summary>
        /// Período analisado em meses (ex: 6 para últimos 6 meses)
        /// </summary>
        public int PeriodoMeses { get; set; }

        /// <summary>
        /// Data inicial da análise
        /// </summary>
        public DateTime DataInicio { get; set; }

        /// <summary>
        /// Data final da análise
        /// </summary>
        public DateTime DataFim { get; set; }

        /// <summary>
        /// Tipo de tendência identificada (Crescente, Decrescente, Estável, Indefinida)
        /// </summary>
        public TipoTendencia TendenciaIdentificada { get; set; }

        /// <summary>
        /// Lista de dados mensais consolidados para exibição em gráficos
        /// </summary>
        public List<DadosMensalViewModel> DadosMensais { get; set; } = new List<DadosMensalViewModel>();

        /// <summary>
        /// Saldo total atual (último mês analisado)
        /// </summary>
        public double SaldoAtual { get; set; }

        /// <summary>
        /// Média mensal de receitas no período
        /// </summary>
        public double MediaReceitas { get; set; }

        /// <summary>
        /// Média mensal de despesas no período
        /// </summary>
        public double MediaDespesas { get; set; }

        /// <summary>
        /// Média mensal de saldo no período
        /// </summary>
        public double MediaSaldo { get; set; }

        /// <summary>
        /// Variação percentual entre o primeiro e último mês
        /// Indica o crescimento ou redução geral do período
        /// </summary>
        public double VariacaoPercentualTotal { get; set; }

        /// <summary>
        /// Mensagem textual descrevendo a tendência identificada
        /// Ex: "Suas finanças apresentam uma tendência crescente de 15,3%"
        /// </summary>
        public string MensagemTendencia { get; set; } = string.Empty;

        /// <summary>
        /// Lista de alertas identificados (ex: gastos atípicos, quedas bruscas)
        /// </summary>
        public List<string> Alertas { get; set; } = new List<string>();

        /// <summary>
        /// Indica se há dados suficientes para análise confiável
        /// Mínimo recomendado: 3 meses
        /// </summary>
        public bool DadosSuficientes { get; set; }

        /// <summary>
        /// Mês com maior saldo positivo no período
        /// </summary>
        public string? MelhorMes { get; set; }

        /// <summary>
        /// Mês com menor saldo (ou maior prejuízo) no período
        /// </summary>
        public string? PiorMes { get; set; }
    }
}