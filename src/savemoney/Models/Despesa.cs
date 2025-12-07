using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Despesa")]
    public class Despesa
    {
        public Despesa()
        {
            DataInicio = DateTime.Today;
            DataFim = DateTime.Today;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da despesa é obrigatório.")]
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Column(TypeName = "decimal(18,3)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        public string CurrencyType { get; set; } = "BRL";

        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        // FK para BudgetCategory (mantido para orçamentos)
        public int? BudgetCategoryId { get; set; }

        [ForeignKey("BudgetCategoryId")]
        public BudgetCategory? BudgetCategory { get; set; }

        // FK para Category (nova - link direto)
        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public bool Pago { get; set; }

        [Display(Name = "É Recorrente?")]
        public bool IsRecurring { get; set; }

        public RecurrenceType Recurrence { get; set; }

        [Display(Name = "Quantidade de Recorrencias")]
        public int? RecurrenceCount { get; set; }

        // FK para Usuario
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        public enum RecurrenceType
        {
            [Display(Name = "Diária")]
            Daily,
            [Display(Name = "Semanal")]
            Weekly,
            [Display(Name = "Mensal")]
            Monthly,
            [Display(Name = "Anual")]
            Yearly
        }
    }
}