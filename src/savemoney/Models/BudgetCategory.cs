using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("BudgetCategory")]
    public class BudgetCategory
    {
        public BudgetCategory()
        {
            // construtor vazio obrigatório para o binder
        }

        [Key]
        public int Id { get; set; }


        public int BudgetId { get; set; }


        public int CategoryId { get; set; }


        [Range(0.01, double.MaxValue, ErrorMessage = "O limite deve ser maior que zero.")]
        public decimal Limit { get; set; }

        // Propriedade calculada para o total gasto (dinâmica, via consulta SQL)
        // Total gasto (calculado dinamicamente)
        [NotMapped]
        public decimal CurrentSpent { get; set; } // Será preenchido via query

        // Propriedades de navegação
        [ForeignKey("BudgetId")]
        [BindNever]
        public virtual Budget Budget { get; set; } = null!;

        [ForeignKey("CategoryId")]
        [BindNever]
        public virtual Category Category { get; set; } = null!;

        //public virtual ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();


        private decimal CalculateCurrentSpent()
        {
            // Placeholder: Somar despesas associadas quando Expense for implementada
            // Exemplo: return DbContext.Expenses.Where(e => e.BudgetCategoryId == Id).Sum(e => e.Amount);
            return 0;
        }
    }
}