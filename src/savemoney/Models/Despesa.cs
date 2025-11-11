using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace savemoney.Models
{
    public class Despesa : IRecurring
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome da despesa é obrigatório.")]
        public string Titulo { get; set; } = null!;
        [Required(ErrorMessage = "Valor da despesa é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Informe um valor maior que Zero (0)")]
        public double Valor { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; }
        [Display(Name = "Categoria")]
        public int? BudgetCategoryId { get; set; }
        [Display(Name = "Categoria")]
        public virtual BudgetCategory? BudgetCategory { get; set; }
        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }
        // Campos de recorrência

        [Display(Name = "É Recorrênte?")]
        public bool IsRecurring { get; set; } = false;

        [Display(Name = "frequência")]
        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.None;
        public int Interval { get; set; } = 1;

        [Display(Name = "Tempo de Recorrência")]
        public DateTime? RecurrenceEndDate { get; set; }

        [Display(Name = "Ocorrências de Recorrência")]
        public int? RecurrenceOccurrences { get; set; }
        [NotMapped]
        public int GeneratedOccurrences { get; set; }
    }
}
