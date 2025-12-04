using System.ComponentModel.DataAnnotations;

namespace savemoney.Models.Enums
{
    /// <summary>
    /// Classificação da categoria para cálculo do DRE Gerencial.
    /// Define se a categoria representa custo variável ou despesa operacional.
    /// </summary>
    public enum TipoClassificacaoDre
    {
        [Display(Name = "Não Classificado")]
        NaoClassificado = 0,

        [Display(Name = "Custo Variável")]
        CustoVariavel = 1,

        [Display(Name = "Despesa Operacional")]
        DespesaOperacional = 2
    }
}