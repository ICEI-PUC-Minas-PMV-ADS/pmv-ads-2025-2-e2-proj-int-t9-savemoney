using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Budget")]
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do orçamento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do orçamento deve ter no máximo 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Description { get; set; } // Novo campo, opcional

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A data de término é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public int UserId { get; set; } // Confirmado como int, baseado no modelo Usuario

        public BudgetStatus Status { get; set; }

        // Relacionamentos
        [ForeignKey("UserId")]
        public virtual Usuario Usuario { get; set; } = null!;

        public virtual ICollection<BudgetCategory> Categories { get; set; } = new List<BudgetCategory>();
    }

    public enum BudgetStatus
    {
        Ativo,
        Concluido,
        Arquivado
    }
}