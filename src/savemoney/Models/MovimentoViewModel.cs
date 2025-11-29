// Models/MovimentoViewModel.cs
namespace savemoney.Models
{
    public class MovimentoViewModel
    {
        public DateTime Data { get; set; }
        public string Descricao { get; set; } = "";
        public decimal Valor { get; set; }
        public string Tipo { get; set; } = ""; // "Entrada" ou "Saída"
        public string Icone { get; set; } = "";
    }
}