using System.ComponentModel.DataAnnotations;

namespace savemoney.Models
{
    public enum RecurrenceType
    {
        [Display(Name = "Diária")]
        Diario,
        
        [Display(Name = "Semanal")]
        Semanal,
        
        [Display(Name = "Mensal")]
        Mensal,
        
        [Display(Name = "Anual")]
        Anual,
        
        // Manter compatibilidade com código antigo em inglês
        Daily = Diario,
        Weekly = Semanal,
        Monthly = Mensal,
        Yearly = Anual
    }
}