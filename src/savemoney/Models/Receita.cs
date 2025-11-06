using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Receitas")]
    public class Receita : IRecurring
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da receita é obrigatório.")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "Valor da receita é obrigatório.")]
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
        public bool IsRecurring { get; set; } = false;
        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.None;
        public int Interval { get; set; } = 1;
        public DateTime? RecurrenceEndDate { get; set; }
        public int? RecurrenceOccurrences { get; set; }

        [NotMapped]
        public int GeneratedOccurrences { get; set; }
    }
}
