namespace SaveMoney.Models.ViewModels
{
    public class ProjecaoViewModel
    {
        // Cards de Resumo
        public decimal SaldoAtual { get; set; }
        public decimal SaldoProjetado6Meses { get; set; }
        public decimal FluxoMensal { get; set; }

        // Dados para o Gráfico (Eixo X e Eixo Y)
        public List<string> Meses { get; set; } = new List<string>();
        public List<decimal> Saldos { get; set; } = new List<decimal>();
    }
}