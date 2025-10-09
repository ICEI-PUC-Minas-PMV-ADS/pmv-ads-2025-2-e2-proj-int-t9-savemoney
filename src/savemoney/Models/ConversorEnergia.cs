using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public class ConversorEnergia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Valor Base é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Informe um valor base maior que zero.")]
        public double ValorBase { get; set; }

        [Required(ErrorMessage = "O campo Tipo do Valor é obrigatório.")]
        [Display(Name = "Tipo do Valor")]
        public string TipoValor { get; set; } = string.Empty;

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

        [Display(Name = "Tempo de Uso")]
        public string? TempoUso { get; set; }

        [Display(Name = "Consumo Mensal (kWh)")]
        [Range(0, double.MaxValue, ErrorMessage = "O consumo mensal deve ser igual ou maior que zero.")]
        public double? ConsumoMensal { get; set; }
    }
}