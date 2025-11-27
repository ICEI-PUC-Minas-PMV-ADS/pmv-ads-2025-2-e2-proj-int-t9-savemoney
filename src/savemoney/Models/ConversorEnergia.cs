using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public class ConversorEnergia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Valor Base é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Informe um valor base maior que zero.")]
        [Display(Name = "Valor Base")]
        public double ValorBase { get; set; }

        [Required(ErrorMessage = "O campo Tipo do Valor é obrigatório.")]
        [Display(Name = "Tipo do Valor")]
        public string TipoValor { get; set; } = string.Empty; // "Watts" ou "Reais"

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Modalidade é obrigatório.")]
        public string Modalidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Bandeira Tarifária é obrigatório.")]
        [Display(Name = "Bandeira Tarifária")]
        public string BandeiraTarifaria { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Tipo de Dispositivo é obrigatório.")]
        [Display(Name = "Tipo de Dispositivo")]
        public string TipoDispositivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Tempo de Uso é obrigatório.")]
        [Range(0.1, 24, ErrorMessage = "Tempo deve estar entre 0.1 e 24 horas")]
        [Display(Name = "Tempo de Uso (horas/dia)")]
        public double TempoUso { get; set; }

        [Display(Name = "Consumo Mensal (kWh)")]
        public double? ConsumoMensal { get; set; }

        [Display(Name = "Custo Mensal Estimado (R$)")]
        public double? CustoMensal { get; set; }
    }
}