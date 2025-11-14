using System;

namespace savemoney.Models
{
    /// <summary>
    /// ViewModel que representa os dados financeiros agregados de um mês específico
    /// Usado para consolidar receitas e despesas em um único objeto
    /// </summary>
    public class DadosMensalViewModel
    {
        /// <summary>
        /// Mês/Ano de referência (ex: "Jan/2024")
        /// </summary>
        public string MesAno { get; set; } = string.Empty;

        /// <summary>
        /// Data de referência (primeiro dia do mês)
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Total de receitas do mês
        /// </summary>
        public double TotalReceitas { get; set; }

        /// <summary>
        /// Total de despesas do mês
        /// </summary>
        public double TotalDespesas { get; set; }

        /// <summary>
        /// Saldo do mês (Receitas - Despesas)
        /// </summary>
        public double Saldo => TotalReceitas - TotalDespesas;

        /// <summary>
        /// Percentual de variação em relação ao mês anterior
        /// Null se não houver mês anterior para comparação
        /// </summary>
        public double? VariacaoPercentual { get; set; }

        /// <summary>
        /// Indica se este mês apresenta um valor atípico (outlier)
        /// Útil para destacar meses com gastos/receitas anormais
        /// </summary>
        public bool IsOutlier { get; set; }
    }
}