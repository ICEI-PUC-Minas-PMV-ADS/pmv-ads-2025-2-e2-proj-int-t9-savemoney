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

            Console.WriteLine($"🔍 Buscando dados de {inicio:dd/MM/yyyy} até {fim:dd/MM/yyyy} ({meses} meses)");

            // Buscar receitas e despesas do período (sem UsuarioId)
            var receitas = await _context.Receitas
                .Where(r => r.DataInicio >= inicio && r.DataInicio <= fim)
                .OrderBy(r => r.DataInicio)
                .ToListAsync();

            var despesas = await _context.Despesas
                .Where(d => d.DataInicio >= inicio && d.DataInicio <= fim)
                .OrderBy(d => d.DataInicio)
                .ToListAsync();

            Console.WriteLine($"📊 Encontradas {receitas.Count} receitas e {despesas.Count} despesas");

            // Verificar se há dados suficientes
            var dadosSuficientes = receitas.Any() || despesas.Any();

            if (!dadosSuficientes)
            {
                return new RelatorioTendenciaViewModel
                {
                    PeriodoMeses = 0,
                    DataInicio = inicio,
                    DataFim = fim,
                    DadosSuficientes = false,
                    MensagemTendencia = "Não há dados suficientes para gerar análise de tendências.",
                    Alertas = new List<string>
                    {
                        "Comece registrando suas receitas e despesas para obter insights financeiros."
                    },
                    DadosMensais = new List<DadosMensalViewModel>()
                };
            }

            // Calcular todos os meses do período
            var todosDadosMensais = CalcularDadosMensais(receitas, despesas, inicio, fim);

            // Filtrar apenas meses com movimentação real
            var dadosMensaisReais = todosDadosMensais
                .Where(d => d.TotalReceitas > 0 || d.TotalDespesas > 0)
                .ToList();

            Console.WriteLine($"📅 Meses com dados reais: {dadosMensaisReais.Count}");

            // Se não há meses com dados reais
            if (dadosMensaisReais.Count == 0)
            {
                return new RelatorioTendenciaViewModel
                {
                    PeriodoMeses = 0,
                    DataInicio = inicio,
                    DataFim = fim,
                    DadosSuficientes = false,
                    MensagemTendencia = "Não há dados suficientes para gerar análise de tendências.",
                    Alertas = new List<string>
                    {
                        "Comece registrando suas receitas e despesas para obter insights financeiros."
                    },
                    DadosMensais = new List<DadosMensalViewModel>()
                };
            }

            // Recalcular variações apenas entre meses consecutivos com dados
            RecalcularVariacoes(dadosMensaisReais);

            // Período real analisado
            var dataInicioReal = dadosMensaisReais.First().Data;
            var dataFimReal = dadosMensaisReais.Last().Data;
            var periodoReal = dadosMensaisReais.Count;

            Console.WriteLine($"✅ Período real: {dataInicioReal:MMM/yyyy} - {dataFimReal:MMM/yyyy} ({periodoReal} meses)");

            // Calcular variação total apenas se houver 2+ meses
            var variacaoTotal = 0.0;
            if (periodoReal >= 2)
            {
                var primeiroSaldo = dadosMensaisReais.First().Saldo;
                var ultimoSaldo = dadosMensaisReais.Last().Saldo;

                if (primeiroSaldo != 0)
                {
                    variacaoTotal = ((ultimoSaldo - primeiroSaldo) / Math.Abs(primeiroSaldo)) * 100;
                }
            }

            // Identificar tendência
            var tendencia = _calculadora.IdentificarTendencia(variacaoTotal);

            // Gerar alertas inteligentes (só se tiver 3+ meses)
            var alertas = GerarAlertasInteligentes(dadosMensaisReais, periodoReal);

            // Calcular totais
            var totalReceitas = receitas.Sum(r => (double)r.Valor);
            var totalDespesas = despesas.Sum(d => (double)d.Valor);
            var saldoAtual = totalReceitas - totalDespesas;

            // Calcular médias
            var mediaReceitas = dadosMensaisReais.Average(d => d.TotalReceitas);
            var mediaDespesas = dadosMensaisReais.Average(d => d.TotalDespesas);
            var mediaSaldo = dadosMensaisReais.Average(d => d.Saldo);

            // Melhor e pior mês (apenas se tiver dados suficientes)
            var melhorMes = periodoReal >= 2 ? dadosMensaisReais.OrderByDescending(d => d.Saldo).First() : null;
            var piorMes = periodoReal >= 2 ? dadosMensaisReais.OrderBy(d => d.Saldo).First() : null;

            // Detectar outliers (só se tiver 4+ meses)
            if (periodoReal >= 4)
            {
                DetectarOutliers(dadosMensaisReais);
            }

            // Gerar mensagem de tendência
            var mensagemTendencia = GerarMensagemTendencia(tendencia, variacaoTotal, periodoReal);

            Console.WriteLine($"✅ Análise concluída: {tendencia} ({variacaoTotal:F1}%)");

            return new RelatorioTendenciaViewModel
            {
                PeriodoMeses = periodoReal,
                DataInicio = dataInicioReal,
                DataFim = dataFimReal,
                TendenciaIdentificada = tendencia,
                DadosMensais = dadosMensaisReais,
                SaldoAtual = saldoAtual,
                MediaReceitas = mediaReceitas,
                MediaDespesas = mediaDespesas,
                MediaSaldo = mediaSaldo,
                VariacaoPercentualTotal = variacaoTotal,
                MensagemTendencia = mensagemTendencia,
                Alertas = alertas,
                DadosSuficientes = true,
                MelhorMes = melhorMes?.MesAno ?? "-",
                PiorMes = piorMes?.MesAno ?? "-"
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

            return dadosMensais;
        }

        private void RecalcularVariacoes(List<DadosMensalViewModel> dadosMensais)
        {
            if (dadosMensais.Count < 2) return;

            // Primeiro mês nunca tem variação
            dadosMensais[0].VariacaoPercentual = null;

            // Calcular variação entre meses consecutivos
            for (int i = 1; i < dadosMensais.Count; i++)
            {
                var mesAnterior = dadosMensais[i - 1].Saldo;
                var mesAtual = dadosMensais[i].Saldo;

                if (mesAnterior != 0)
                {
                    dadosMensais[i].VariacaoPercentual =
                        ((mesAtual - mesAnterior) / Math.Abs(mesAnterior)) * 100;
                }
                else if (mesAtual != 0)
                {
                    // Se mês anterior era zero, qualquer valor é variação infinita
                    dadosMensais[i].VariacaoPercentual = mesAtual > 0 ? 100 : -100;
                }
                else
                {
                    dadosMensais[i].VariacaoPercentual = 0;
                }
            }
        }

        private void DetectarOutliers(List<DadosMensalViewModel> dadosMensais)
        {
            if (dadosMensais.Count < 4) return;

            // Usar IQR (Interquartile Range) para detectar outliers
            var despesas = dadosMensais.Select(d => d.TotalDespesas).OrderBy(d => d).ToList();

            var q1Index = despesas.Count / 4;
            var q3Index = (despesas.Count * 3) / 4;
            var q1 = despesas[q1Index];
            var q3 = despesas[q3Index];
            var iqr = q3 - q1;

            // Outliers são valores fora de [Q1 - 1.5*IQR, Q3 + 1.5*IQR]
            var limiteInferior = q1 - (1.5 * iqr);
            var limiteSuperior = q3 + (1.5 * iqr);

            foreach (var mes in dadosMensais)
            {
                if (mes.TotalDespesas < limiteInferior || mes.TotalDespesas > limiteSuperior)
                {
                    mes.IsOutlier = true;
                }
            }

            Console.WriteLine($"🔍 Outliers detectados: {dadosMensais.Count(d => d.IsOutlier)}");
        }

        private List<string> GerarAlertasInteligentes(List<DadosMensalViewModel> dadosMensais, int mesesComDados)
        {
            var alertas = new List<string>();

            // Se não há dados suficientes para análise
            if (mesesComDados < 2)
            {
                alertas.Add("📊 Continue registrando suas movimentações para obter análises mais precisas.");
                return alertas;
            }

            if (mesesComDados == 2)
            {
                alertas.Add("📊 Com apenas 2 meses de dados, as análises são limitadas. Continue registrando para insights mais precisos.");
            }

            // ALERTAS SÓ COM 3+ MESES
            if (mesesComDados >= 3)
            {
                // Alerta: Gastos acima da média
                var mediaDespesas = dadosMensais.Average(d => d.TotalDespesas);
                var despesasAcimaDaMedia = dadosMensais
                    .Where(d => d.TotalDespesas > mediaDespesas * 1.2)
                    .ToList();

                if (despesasAcimaDaMedia.Any())
                {
                    var meses = string.Join(", ", despesasAcimaDaMedia.Select(d => d.MesAno));
                    alertas.Add($"⚠️ Gastos 20% acima da média em: {meses}");
                }

                // Alerta: Saldos negativos
                var saldosNegativos = dadosMensais.Where(d => d.Saldo < 0).ToList();
                if (saldosNegativos.Any())
                {
                    var meses = string.Join(", ", saldosNegativos.Select(d => d.MesAno));
                    alertas.Add($"🔴 Saldo negativo em: {meses}");
                }

                // Alerta: Queda no último mês
                var ultimoMes = dadosMensais.Last();
                if (ultimoMes.VariacaoPercentual.HasValue && ultimoMes.VariacaoPercentual < -15)
                {
                    alertas.Add($"📉 Queda de {Math.Abs(ultimoMes.VariacaoPercentual.Value):F1}% no último mês");
                }
            }

            // ALERTAS SÓ COM 4+ MESES
            if (mesesComDados >= 4)
            {
                // Alerta: Outliers
                var outliers = dadosMensais.Where(d => d.IsOutlier).ToList();
                if (outliers.Any())
                {
                    var meses = string.Join(", ", outliers.Select(d => d.MesAno));
                    alertas.Add($"⚡ Gastos atípicos detectados em: {meses}");
                }
            }

            // Se não há alertas, dar feedback positivo
            if (alertas.Count == 0)
            {
                alertas.Add("✅ Suas finanças estão em equilíbrio. Continue assim!");
            }

            return alertas;
        }

        private string GerarMensagemTendencia(TipoTendencia tendencia, double variacao, int mesesComDados)
        {
            // Se tem menos de 2 meses, não há tendência
            if (mesesComDados < 2)
            {
                return "Continue registrando suas movimentações para identificar tendências financeiras.";
            }

            // Se tem 2 meses, avisar que é preliminar
            if (mesesComDados == 2)
            {
                return $"Com base nos 2 meses analisados, suas finanças apresentam uma variação de {(variacao >= 0 ? "+" : "")}{variacao:F1}%. Continue registrando para análises mais precisas.";
            }

            // Com 3+ meses, análise normal
            return tendencia switch
            {
                TipoTendencia.Crescente => $"✅ Suas finanças apresentam uma tendência crescente de {variacao:F1}%",
                TipoTendencia.Decrescente => $"⚠️ Suas finanças apresentam uma tendência decrescente de {Math.Abs(variacao):F1}%",
                TipoTendencia.Estavel => "📊 Suas finanças apresentam uma tendência estável",
                _ => "Não foi possível identificar uma tendência clara"
            };
        }
    }
}