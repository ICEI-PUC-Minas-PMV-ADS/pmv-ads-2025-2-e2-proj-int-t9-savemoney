using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    /// <summary>
    /// Define a granularidade da visualização do relatório financeiro
    /// </summary>
    public enum PeriodoAgregacao
    {
        /// <summary>
        /// Agrupa transações por dia
        /// </summary>
        [Display(Name = "Diário")]
        Diario,

        /// <summary>
        /// Agrupa transações por semana
        /// </summary>
        [Display(Name = "Semanal")]
        Semanal,

        /// <summary>
        /// Agrupa transações por mês
        /// </summary>
        [Display(Name = "Mensal")]
        Mensal,

        /// <summary>
        /// Agrupa transações por trimestre (3 meses)
        /// </summary>
        [Display(Name = "Trimestral")]
        Trimestral,

        /// <summary>
        /// Agrupa transações por semestre (6 meses)
        /// </summary>
        [Display(Name = "Semestral")]
        Semestral,

        /// <summary>
        /// Agrupa transações por ano
        /// </summary>
        [Display(Name = "Anual")]
        Anual
    }
}