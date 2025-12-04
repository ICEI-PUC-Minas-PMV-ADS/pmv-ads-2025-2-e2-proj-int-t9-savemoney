using System.ComponentModel.DataAnnotations;

namespace savemoney.Models.Enums
{
    /// <summary>
    /// Classificação contábil da categoria.
    /// Usado para organização e cálculos financeiros (DRE, relatórios).
    /// </summary>
    public enum TipoContabil
    {
        [Display(Name = "Não Classificado")]
        NaoClassificado = 0,

        [Display(Name = "Receita")]
        Receita = 1,

        [Display(Name = "Custo Variável")]
        CustoVariavel = 2,

        [Display(Name = "Despesa Fixa")]
        DespesaFixa = 3,

        [Display(Name = "Despesa Operacional")]
        DespesaOperacional = 4,

        [Display(Name = "Investimento")]
        Investimento = 5
    }
}