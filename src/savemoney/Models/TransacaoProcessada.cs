using System;

namespace savemoney.Models
{
    // Objeto simplificado para exibição em relatórios e gráficos
    public class TransacaoProcessada
    {
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } // Título da receita/despesa
        public string Categoria { get; set; } // Nome da categoria
        public bool EhReceita { get; set; }   // True = Receita, False = Despesa
    }
}