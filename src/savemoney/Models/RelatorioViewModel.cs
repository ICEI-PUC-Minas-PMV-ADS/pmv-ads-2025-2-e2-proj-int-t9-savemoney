using System;
using System.Collections.Generic;
using System.Linq;

namespace savemoney.Models
{
    public class PeriodoViewModel
    {
        public string Label { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public double TotalReceitas { get; set; }
        public double TotalDespesas { get; set; }
        public double Saldo => TotalReceitas - TotalDespesas;
        public double? VariacaoPercentual { get; set; }
    }

    public class RelatorioViewModel
    {
        public RelatorioRequest Request { get; set; } = new RelatorioRequest();
        public List<PeriodoViewModel> Periodos { get; set; } = new List<PeriodoViewModel>();
        public double TotalReceitas => Periodos.Sum(p => p.TotalReceitas);
        public double TotalDespesas => Periodos.Sum(p => p.TotalDespesas);
        public double Saldo => TotalReceitas - TotalDespesas;
    }
}