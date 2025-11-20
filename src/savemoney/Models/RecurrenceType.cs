using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public enum Recurrence
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
}