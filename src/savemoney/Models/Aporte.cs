using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace savemoney.Models
{
    [Table("Aportes")]
    public class Aporte
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O valor do aporte é obrigatório.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Valor do Aporte")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do aporte deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data do aporte é obrigatória.")]
        [Display(Name = "Data do Aporte")]
        [DataType(DataType.Date)]
        public DateTime DataAporte { get; set; }

        // Chave estrangeira para a Meta Financeira
        [Required]
        public int MetaFinanceiraId { get; set; }

        [ForeignKey("MetaFinanceiraId")]
        public MetaFinanceira? MetaFinanceira { get; set; }
    }
}