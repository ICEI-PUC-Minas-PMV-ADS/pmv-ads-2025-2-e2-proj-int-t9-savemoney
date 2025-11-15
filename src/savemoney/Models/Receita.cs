using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public class Receita
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        [StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Display(Name = "Tipo de Moeda")]
        public string CurrencyType { get; set; } = "BRL";

        [DataType(DataType.Date)]
        [Display(Name = "Data Início")]
        public DateTime DataInicio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data Fim")]
        public DateTime DataFim { get; set; }

        [Display(Name = "Recebido?")]
        public bool Recebido { get; set; }

        [Display(Name = "Recorrente?")]
        public bool IsRecurring { get; set; }

        [Display(Name = "Tipo de Recorrência")]
        public RecurrenceType Recurrence { get; set; }
        public enum RecurrenceType
        {
            Daily,
            Weekly,
            Monthly,
            Yearly
        }

        [Display(Name = "Número de Repetições")]
        [Range(1, 365, ErrorMessage = "Informe entre 1 e 365 repetições.")]
        public int? RecurrenceCount { get; set; }
    }
}
