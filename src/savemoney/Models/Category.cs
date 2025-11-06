using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da categoria deve ter no máximo 100 caracteres.")]
        public required string Name { get; set; }

        public bool IsPredefined { get; set; } // true = sistema, false = usuário

        // Apenas para categorias personalizadas
        public int? UsuarioId { get; set; }
        public virtual Usuario? Usuario { get; set; }

        // Relacionamento N:N com Budget via BudgetCategory
        public virtual ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();
    }
}