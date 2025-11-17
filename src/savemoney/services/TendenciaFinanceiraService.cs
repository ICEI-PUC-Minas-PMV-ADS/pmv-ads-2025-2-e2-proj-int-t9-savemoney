using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services.Helpers;
using savemoney.Services.Interfaces;

namespace savemoney.Services
{
    /// <summary>
    /// Implementação CORRIGIDA do serviço de análise de tendências financeiras
    /// Adaptado para os novos models com DataInicio/DataFim e Valor decimal
    /// </summary>
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
            // Validação de entrada
            if (meses < 1 || meses > 12)
                throw new ArgumentException("O período deve estar entre 1 e 12 meses.");

            // Define o período de análise
            var dataFim = DateTime.Today;
            var dataInicio = dataFim.AddMonths(-meses);

            // Busca dados do banco de dados
            var receitas = await BuscarReceitasPorPeriodoAsync(usuarioId, dataInicio, dataFim);
            var despesas = await BuscarDespesasPorPeriodoAsync(usuarioId, dataInicio, dataFim);

            // Processa dados mensais
            var dadosMensais = ProcessarDadosMensais(receitas, despesas, dataInicio, dataFim);

            // Verifica se há dados suficientes
            if (dadosMensais.Count < 2)
            {
                return CriarRelatorioSemDados(meses, dataInicio, dataFim);
            }

            // Calcula métricas e tendências
            var relatorio = ConstruirRelatorio(dadosMensais, meses, dataInicio, dataFim);

            return relatorio;
        }

        /// <summary>
        /// Busca receitas do usuário no período especificado
        /// CORREÇÃO: Usa DataInicio como referência e Valor decimal
        /// </summary>
        private async Task<List<Receita>> BuscarReceitasPorPeriodoAsync(int usuarioId, DateTime inicio, DateTime fim)
        {
            // TODO: Adicionar filtro por UsuarioId quando a coluna for criada
            // Por enquanto, busca todas as receitas do período

            return await _context.Receitas
                .AsNoTracking()
                .Where(r => r.DataInicio >= inicio && r.DataInicio <= fim)
                .OrderBy(r => r.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Busca despesas do usuário no período especificado
        /// CORREÇÃO: Usa DataInicio como referência
        /// </summary>
        private async Task<List<Despesa>> BuscarDespesasPorPeriodoAsync(int usuarioId, DateTime inicio, DateTime fim)
        {
            // TODO: Adicionar filtro por UsuarioId quando a coluna for criada

            // CORREÇÃO: AppDbContext tem DbSet<Despesa> chamado "Despesa" (singular)
            return await _context.Set<Despesa>()
                .AsNoTracking()
                .Where(d => d.DataInicio >= inicio && d.DataInicio <= fim)
                .OrderBy(d => d.DataInicio)
                .ToListAsync();
        }

        /// <summary>
        /// Agrupa receitas e despesas por mês e calcula totais
        /// CORREÇÃO: Usa DataInicio e converte decimal para double
        /// </summary>
        private List<DadosMensalViewModel> ProcessarDadosMensais(
            List<Receita> receitas,
            List<Despesa> despesas,
            DateTime inicio,
            DateTime fim)
        {
            var dadosMensais = new List<DadosMensalViewModel>();

            // Itera por cada mês do período
            var dataAtual = new DateTime(inicio.Year, inicio.Month, 1);
            var dataFimMes = new DateTime(fim.Year, fim.Month, 1);

            while (dataAtual <= dataFimMes)
            {
                var mesAtual = dataAtual.Month;
                var anoAtual = dataAtual.Year;

                // Filtra receitas do mês e converte decimal para double
                var receitasMes = receitas
                    .Where(r => r.DataInicio.Year == anoAtual && r.DataInicio.Month == mesAtual)
                    .Sum(r => (double)r.Valor); // Conversão decimal -> double

                // Filtra despesas do mês
                var despesasMes = despesas
                    .Where(d => d.DataInicio.Year == anoAtual && d.DataInicio.Month == mesAtual)
                    .Sum(d => (double)d.Valor); // Conversão decimal -> double

                dadosMensais.Add(new DadosMensalViewModel
                {
                    Data = dataAtual,
                    MesAno = dataAtual.ToString("MMM/yyyy"), // Ex: "Jan/2024"
                    TotalReceitas = receitasMes,
                    TotalDespesas = despesasMes
                });

                dataAtual = dataAtual.AddMonths(1);
            }

            // Calcula variações percentuais
            for (int i = 1; i < dadosMensais.Count; i++)
            {
                var saldoAnterior = dadosMensais[i - 1].Saldo;
                var saldoAtual = dadosMensais[i].Saldo;

                dadosMensais[i].VariacaoPercentual =
                    _calculadora.CalcularVariacaoPercentual(saldoAnterior, saldoAtual);
            }

            // Identifica outliers
            var saldos = dadosMensais.Select(d => d.Saldo).ToList();
            var indicesOutliers = _calculadora.IdentificarOutliers(saldos);

            foreach (var indice in indicesOutliers)
            {
                dadosMensais[indice].IsOutlier = true;
            }

            return dadosMensais;
        }

        /// <summary>
        /// Constrói o relatório completo com todas as métricas e análises
        /// </summary>
        private RelatorioTendenciaViewModel ConstruirRelatorio(
            List<DadosMensalViewModel> dadosMensais,
            int meses,
            DateTime inicio,
            DateTime fim)
        {
            var primeiroMes = dadosMensais.First();
            var ultimoMes = dadosMensais.Last();

            // Calcula variação total do período
            var variacaoTotal = _calculadora.CalcularVariacaoPercentual(
                primeiroMes.Saldo,
                ultimoMes.Saldo
            );

            // Identifica tipo de tendência
            var tendencia = _calculadora.IdentificarTendencia(variacaoTotal);

            // Calcula médias
            var mediaReceitas = dadosMensais.Average(d => d.TotalReceitas);
            var mediaDespesas = dadosMensais.Average(d => d.TotalDespesas);
            var mediaSaldo = dadosMensais.Average(d => d.Saldo);

            // Identifica melhor e pior mês
            var melhorMes = dadosMensais.OrderByDescending(d => d.Saldo).First();
            var piorMes = dadosMensais.OrderBy(d => d.Saldo).First();

            // Gera alertas
            var alertas = GerarAlertas(dadosMensais, mediaSaldo);

            var relatorio = new RelatorioTendenciaViewModel
            {
                PeriodoMeses = meses,
                DataInicio = inicio,
                DataFim = fim,
                TendenciaIdentificada = tendencia,
                DadosMensais = dadosMensais,
                SaldoAtual = ultimoMes.Saldo,
                MediaReceitas = mediaReceitas,
                MediaDespesas = mediaDespesas,
                MediaSaldo = mediaSaldo,
                VariacaoPercentualTotal = variacaoTotal,
                MensagemTendencia = _calculadora.GerarMensagemTendencia(tendencia, variacaoTotal),
                Alertas = alertas,
                DadosSuficientes = true,
                MelhorMes = melhorMes.MesAno,
                PiorMes = piorMes.MesAno
            };

            return relatorio;
        }

        /// <summary>
        /// Gera lista de alertas baseados em análise dos dados
        /// </summary>
        private List<string> GerarAlertas(List<DadosMensalViewModel> dados, double mediaSaldo)
        {
            var alertas = new List<string>();

            // Alerta de outliers
            var outliers = dados.Where(d => d.IsOutlier).ToList();
            foreach (var outlier in outliers)
            {
                if (outlier.Saldo > mediaSaldo)
                    alertas.Add($"⚠️ {outlier.MesAno}: Saldo excepcionalmente alto (R$ {outlier.Saldo:N2})");
                else
                    alertas.Add($"🔴 {outlier.MesAno}: Saldo excepcionalmente baixo (R$ {outlier.Saldo:N2})");
            }

            // Alerta de variações bruscas
            var variacoesBruscas = dados
                .Where(d => d.VariacaoPercentual.HasValue && Math.Abs(d.VariacaoPercentual.Value) > 30)
                .ToList();

            foreach (var mes in variacoesBruscas)
            {
                alertas.Add($"📊 {mes.MesAno}: Variação brusca de {mes.VariacaoPercentual:F1}%");
            }

            // Alerta de saldo negativo
            var mesesNegativos = dados.Where(d => d.Saldo < 0).ToList();
            if (mesesNegativos.Any())
            {
                alertas.Add($"💰 {mesesNegativos.Count} mês(es) com saldo negativo identificado(s)");
            }

            return alertas;
        }

        /// <summary>
        /// Cria um relatório indicando falta de dados
        /// </summary>
        private RelatorioTendenciaViewModel CriarRelatorioSemDados(int meses, DateTime inicio, DateTime fim)
        {
            return new RelatorioTendenciaViewModel
            {
                PeriodoMeses = meses,
                DataInicio = inicio,
                DataFim = fim,
                TendenciaIdentificada = TipoTendencia.Indefinida,
                DadosMensais = new List<DadosMensalViewModel>(),
                SaldoAtual = 0,
                MediaReceitas = 0,
                MediaDespesas = 0,
                MediaSaldo = 0,
                VariacaoPercentualTotal = 0,
                MensagemTendencia = "Não há dados suficientes para análise. Registre receitas e despesas para começar.",
                Alertas = new List<string> { "⚠️ Adicione pelo menos 2 meses de dados para análise confiável." },
                DadosSuficientes = false
            };
        }
    }
}