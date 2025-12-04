using Microsoft.AspNetCore.Mvc.ModelBinding;
using savemoney.Models.Enums;
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

        public bool IsPredefined { get; set; }

        /// <summary>
        /// Classificação para o DRE Gerencial.
        /// Define se é Custo Variável ou Despesa Operacional.
        /// </summary>
        [Display(Name = "Classificação DRE")]
        public TipoClassificacaoDre ClassificacaoDre { get; set; } = TipoClassificacaoDre.NaoClassificado;

        public int? UsuarioId { get; set; }

        [BindNever]
        public virtual Usuario? Usuario { get; set; }

        [BindNever]
        public virtual ICollection<BudgetCategory> BudgetCategories { get; set; } = new List<BudgetCategory>();
    }
}