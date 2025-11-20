using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public enum Recurrence
    {
        [Display(Name = "Diária")]
        Diario,

        [Display(Name = "Semanal")]
        Semanal,

        [Display(Name = "Mensal")]
        Mensal,

        [Display(Name = "Anual")]
        Anual
    }
}