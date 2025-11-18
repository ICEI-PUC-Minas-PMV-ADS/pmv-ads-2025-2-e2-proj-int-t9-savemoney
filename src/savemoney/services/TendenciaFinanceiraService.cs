using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services.Helpers;
using savemoney.Services.Interfaces;

namespace savemoney.Services
{
    public class TendenciaFinanceiraService : ITendenciaFinanceiraService
    {
        private readonly AppDbContext _context;
        private readonly CalculadoraTendencia _calculadora;

        public TendenciaFinanceiraService(AppDbContext context)
        {
            _context = context;
            _calculadora = new CalculadoraTendencia();
        }

        public async Task<RelatorioTendenciaViewModel> AnalisarTendenciasPorPeriodoAsync(int usuarioId, int meses)
        {
            var fim = DateTime.Now;
            var inicio = fim.AddMonths(-meses);

            Console.WriteLine($"DEBUG: Buscando de {inicio:yyyy-MM-dd} até {fim:yyyy-MM-dd}");

            var receitas = await _context.Receitas
                .Where(r => r.DataInicio >= inicio && r.DataInicio <= fim)
                .OrderBy(r => r.DataInicio)
                .ToListAsync();

            var despesas = await _context.Despesas
                .Where(d => d.DataInicio >= inicio && d.DataInicio <= fim)
                .OrderBy(d => d.DataInicio)
                .ToListAsync();

            Console.WriteLine($"DEBUG: Encontradas {receitas.Count} receitas e {despesas.Count} despesas");

            var dadosSuficientes = receitas.Any() || despesas.Any();

            if (!dadosSuficientes)
            {
                return new RelatorioTendenciaViewModel
                {
                    PeriodoMeses = meses,
                    DataInicio = inicio,
                    DataFim = fim,
                    DadosSuficientes = false,
                    MensagemTendencia = "Não há dados suficientes para gerar análise de tendências."
                };
            }

            var dadosMensais = CalcularDadosMensais(receitas, despesas, inicio, fim);

            var variacaoTotal = 0.0;
            if (dadosMensais.Count >= 2)
            {
                var primeiroMes = dadosMensais.First().Saldo;
                var ultimoMes = dadosMensais.Last().Saldo;
                if (primeiroMes != 0)
                {
                    variacaoTotal = ((ultimoMes - primeiroMes) / Math.Abs(primeiroMes)) * 100;
                }
            }

            var tendencia = _calculadora.IdentificarTendencia(variacaoTotal);
            var alertas = GerarAlertas(dadosMensais);

            var totalReceitas = receitas.Sum(r => (double)r.Valor);
            var totalDespesas = despesas.Sum(d => (double)d.Valor);
            var saldoAtual = totalReceitas - totalDespesas;

            var mediaReceitas = dadosMensais.Any() ? dadosMensais.Average(d => d.TotalReceitas) : 0;
            var mediaDespesas = dadosMensais.Any() ? dadosMensais.Average(d => d.TotalDespesas) : 0;
            var mediaSaldo = dadosMensais.Any() ? dadosMensais.Average(d => d.Saldo) : 0;

            var melhorMes = dadosMensais.OrderByDescending(d => d.Saldo).FirstOrDefault();
            var piorMes = dadosMensais.OrderBy(d => d.Saldo).FirstOrDefault();

            var mensagemTendencia = GerarMensagemTendencia(tendencia, variacaoTotal);

            return new RelatorioTendenciaViewModel
            {
                PeriodoMeses = meses,
                DataInicio = inicio,
                DataFim = fim,
                TendenciaIdentificada = tendencia,
                DadosMensais = dadosMensais,
                SaldoAtual = saldoAtual,
                MediaReceitas = mediaReceitas,
                MediaDespesas = mediaDespesas,
                MediaSaldo = mediaSaldo,
                VariacaoPercentualTotal = variacaoTotal,
                MensagemTendencia = mensagemTendencia,
                Alertas = alertas,
                DadosSuficientes = dadosSuficientes,
                MelhorMes = melhorMes?.MesAno,
                PiorMes = piorMes?.MesAno
            };
        }

        private List<DadosMensalViewModel> CalcularDadosMensais(
            List<Receita> receitas,
            List<Despesa> despesas,
            DateTime inicio,
            DateTime fim)
        {
            var dadosMensais = new List<DadosMensalViewModel>();
            var mesAtual = new DateTime(inicio.Year, inicio.Month, 1);
            var mesFim = new DateTime(fim.Year, fim.Month, 1);

            while (mesAtual <= mesFim)
            {
                var proximoMes = mesAtual.AddMonths(1);

                var receitasDoMes = receitas
                    .Where(r => r.DataInicio >= mesAtual && r.DataInicio < proximoMes)
                    .Sum(r => (double)r.Valor);

                var despesasDoMes = despesas
                    .Where(d => d.DataInicio >= mesAtual && d.DataInicio < proximoMes)
                    .Sum(d => (double)d.Valor);

                dadosMensais.Add(new DadosMensalViewModel
                {
                    MesAno = mesAtual.ToString("MMM/yyyy"),
                    Data = mesAtual,
                    TotalReceitas = receitasDoMes,
                    TotalDespesas = despesasDoMes,
                    VariacaoPercentual = null,
                    IsOutlier = false
                });

                mesAtual = proximoMes;
            }

            for (int i = 1; i < dadosMensais.Count; i++)
            {
                var mesAnterior = dadosMensais[i - 1].Saldo;
                var mesAtualSaldo = dadosMensais[i].Saldo;

                if (mesAnterior != 0)
                {
                    dadosMensais[i].VariacaoPercentual =
                        ((mesAtualSaldo - mesAnterior) / Math.Abs(mesAnterior)) * 100;
                }
            }

            DetectarOutliers(dadosMensais);

            return dadosMensais;
        }

        private void DetectarOutliers(List<DadosMensalViewModel> dadosMensais)
        {
            if (dadosMensais.Count < 3) return;

            var despesas = dadosMensais.Select(d => d.TotalDespesas).OrderBy(d => d).ToList();
            var q1Index = despesas.Count / 4;
            var q3Index = (despesas.Count * 3) / 4;
            var q1 = despesas[q1Index];
            var q3 = despesas[q3Index];
            var iqr = q3 - q1;
            var limiteInferior = q1 - (1.5 * iqr);
            var limiteSuperior = q3 + (1.5 * iqr);

            foreach (var mes in dadosMensais)
            {
                if (mes.TotalDespesas < limiteInferior || mes.TotalDespesas > limiteSuperior)
                {
                    mes.IsOutlier = true;
                }
            }
        }

        private List<string> GerarAlertas(List<DadosMensalViewModel> dadosMensais)
        {
            var alertas = new List<string>();

            var mediaDespesas = dadosMensais.Average(d => d.TotalDespesas);
            var despesasAcimaDaMedia = dadosMensais
                .Where(d => d.TotalDespesas > mediaDespesas * 1.2)
                .ToList();

            if (despesasAcimaDaMedia.Any())
            {
                var meses = string.Join(", ", despesasAcimaDaMedia.Select(d => d.MesAno));
                alertas.Add($"Gastos 20% acima da média em: {meses}");
            }

            var saldosNegativos = dadosMensais.Where(d => d.Saldo < 0).ToList();
            if (saldosNegativos.Any())
            {
                var meses = string.Join(", ", saldosNegativos.Select(d => d.MesAno));
                alertas.Add($"Saldo negativo em: {meses}");
            }

            var ultimoMes = dadosMensais.LastOrDefault();
            if (ultimoMes?.VariacaoPercentual.HasValue == true && ultimoMes.VariacaoPercentual < -10)
            {
                alertas.Add($"Queda de {Math.Abs(ultimoMes.VariacaoPercentual.Value):F1}% no último mês");
            }

            var outliers = dadosMensais.Where(d => d.IsOutlier).ToList();
            if (outliers.Any())
            {
                var meses = string.Join(", ", outliers.Select(d => d.MesAno));
                alertas.Add($"Gastos atípicos detectados em: {meses}");
            }

            return alertas;
        }

        private string GerarMensagemTendencia(TipoTendencia tendencia, double variacao)
        {
            return tendencia switch
            {
                TipoTendencia.Crescente => $"Suas finanças apresentam uma tendência crescente de {variacao:F1}%",
                TipoTendencia.Decrescente => $"Suas finanças apresentam uma tendência decrescente de {Math.Abs(variacao):F1}%",
                TipoTendencia.Estavel => "Suas finanças apresentam uma tendência estável",
                _ => "Não foi possível identificar uma tendência clara"
            };
        }
    }
}