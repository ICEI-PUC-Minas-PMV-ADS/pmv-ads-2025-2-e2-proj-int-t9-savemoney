using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("BudgetCategory")]
    public class BudgetCategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O orçamento é obrigatório.")]
        public int BudgetId { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "O limite de gastos é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O limite deve ser maior que zero.")]
        public decimal Limit { get; set; }

        // Propriedade calculada para o total gasto (dinâmica, via consulta SQL)
        // Total gasto (calculado dinamicamente)
        [NotMapped]
        public decimal CurrentSpent { get; set; } // Será preenchido via query

        // Propriedades de navegação
        [ForeignKey("BudgetId")]
        public virtual Budget Budget { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;


        public virtual ICollection<Receita>? Receitas { get; set; }
    }
}