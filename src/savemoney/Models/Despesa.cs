using savemoney.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace savemoney.Models
{
    public class Despesa
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da despesa é obrigatório.")]
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        public string CurrencyType { get; set; } = "BRL";

        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        // CHAVE ESTRANGEIRA
        public int? BudgetCategoryId { get; set; }

        // NAVEGAÇÃO
        [ForeignKey("BudgetCategoryId")]
        public BudgetCategory? BudgetCategory { get; set; }

        public bool Recebido { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrenceType Recurrence { get; set; }
        public int? RecurrenceCount { get; set; }

        public enum RecurrenceType
        {
            Daily,
            Weekly,
            Monthly,
            Yearly
        }
    }
}