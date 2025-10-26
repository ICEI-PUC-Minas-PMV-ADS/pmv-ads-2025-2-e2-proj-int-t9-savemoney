using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace savemoney.Models
{
    [Table("MetasFinanceiras")]
    public class MetaFinanceira
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O título da meta é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
        [Display(Name = "Título da Meta")]
        public string Titulo { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O valor objetivo é obrigatório.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Valor Objetivo")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor objetivo deve ser maior que zero.")]
        public decimal ValorObjetivo { get; set; }

        // O ValorAtual é gerenciado pelo sistema (através dos aportes)
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Valor Atual")]
        [DataType(DataType.Currency)]
        public decimal ValorAtual { get; set; }

        [Display(Name = "Data Limite (Opcional)")]
        [DataType(DataType.Date)]
        public DateTime? DataLimite { get; set; }

        // Chave estrangeira para o Usuário (Assumindo que você já possui a classe Usuario)
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        // Propriedade de navegação para os Aportes
        public ICollection<Aporte>? Aportes { get; set; }

        // Propriedades calculadas (não salvas no banco de dados)
        [NotMapped]
        [Display(Name = "Progresso")]
        public double Progresso
        {
            get
            {
                if (ValorObjetivo <= 0) return 0;
                // Calcula a proporção (0.0 a 1.0)
                double progresso = (double)(ValorAtual / ValorObjetivo);
                return Math.Min(progresso, 1.0); // Limita o progresso a 100% (1.0)
            }
        }

        [NotMapped]
        [Display(Name = "Concluída")]
        public bool EstaConcluida
        {
            get
            {
                return ValorAtual >= ValorObjetivo;
            }
        }
    }
}