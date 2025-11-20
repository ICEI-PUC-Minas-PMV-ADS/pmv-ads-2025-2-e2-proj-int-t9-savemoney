using savemoney.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace savemoney.Models
{
    [Table("Receita")]
    public class Receita
    {
        public Receita()
        {
            DataInicio = DateTime.Today;
            DataFim = DateTime.Today;
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Column(TypeName = "decimal(18,3)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        [RegularExpression(@"^\d+(\,\d{1,3})?$", ErrorMessage = "Máximo 3 casas decimais.")]
        public decimal Valor { get; set; }

        [Display(Name = "Tipo de Moeda")]
        public string CurrencyType { get; set; } = "BRL";

        [DataType(DataType.Date)]
        [Display(Name = "Data Início")]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Fim")]
        public DateTime DataFim { get; set; }

        [Display(Name = "Recebido")]
        public bool Recebido { get; set; }

        [Display(Name = "É Recorrente?")]
        public bool IsRecurring { get; set; }

        [Display(Name = "Tipo de Recorrência")]
        public RecurrenceType Recurrence { get; set; }
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

        [Display(Name = "Quantidade de Repetições")]
        [Range(1, 365, ErrorMessage = "Informe entre 1 e 365 repetições.")]
        public int? RecurrenceCount { get; set; }
    }
}
