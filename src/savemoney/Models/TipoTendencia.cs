using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    /// <summary>
    /// Enum que representa os tipos possíveis de tendência financeira
    /// Usado para classificar o comportamento do saldo ao longo do tempo
    /// </summary> 
    public enum TipoTendencia
    {
        [Display(Name = "Crescente")]
        Crescente = 1,

        [Display(Name = "Decrescente")]
        Decrescente = 2,

        [Display(Name = "Estável")]
        Estavel = 3,

        [Display(Name = "Indefinida")]
        Indefinida = 4, // Quando não há dados suficientes para determinar a tendência
    }
}
