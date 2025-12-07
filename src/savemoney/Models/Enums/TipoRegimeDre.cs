using System.ComponentModel.DataAnnotations;

namespace savemoney.Models.Enums
{
    /// <summary>
    /// Define o regime de apuração do DRE Gerencial.
    /// Competência: considera a data do fato gerador (nota fiscal).
    /// Caixa: considera a data do pagamento/recebimento efetivo.
    /// </summary>
    public enum TipoRegimeDre
    {
        [Display(Name = "Competência")]
        Competencia = 0,

        [Display(Name = "Caixa")]
        Caixa = 1
    }
}