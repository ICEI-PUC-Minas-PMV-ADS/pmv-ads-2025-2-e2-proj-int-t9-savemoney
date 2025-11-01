using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [Table("Despesa")]
    public class Despesa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da despesa é obrigatório.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor da despesa é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Informe um valor maior que Zero (0)")]
        public double Valor { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Display(Name = "Categoria")]
        public int BudgetCategoryId { get; set; }
        [Display(Name = "Categoria")]
        public virtual BudgetCategory BudgetCategory { get; set; } = null!;

        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }
    }
}
