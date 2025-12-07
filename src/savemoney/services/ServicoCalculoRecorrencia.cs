using savemoney.Models;
using savemoney.Controllers;
using System.Globalization;

namespace savemoney.Services
{
    /// <summary>
    /// Serviço responsável por processar transações recorrentes e gerar análises financeiras
    /// </summary>
    public static class ServicoCalculoRecorrencia
    {
        #region Processamento Principal

        /// <summary>
        /// Processa receitas e despesas, expandindo recorrências para o período especificado
        /// </summary>
        public static List<TransacaoProcessada> ProcessarTransacoes(
            List<ReceitaDTO> receitas,
            List<DespesaDTO> despesas,
            DateTime dataInicioRelatorio,
            DateTime dataFimRelatorio)
        {
            var listaProcessada = new List<TransacaoProcessada>();

            // 1. Processar Receitas
            foreach (var receita in receitas)
            {
                var datasOcorrencia = CalcularDatasOcorrenciaOtimizado(
                    receita.DataInicio,
                    receita.DataFim,
                    receita.IsRecurring,
                    receita.Recurrence,
                    receita.RecurrenceCount,
                    dataInicioRelatorio,
                    dataFimRelatorio
                );

                foreach (var data in datasOcorrencia)
                {
                    listaProcessada.Add(new TransacaoProcessada
                    {
                        Data = data,
                        Valor = receita.Valor,
                        Descricao = receita.Titulo ?? "Receita",
                        Categoria = "Receita",
                        EhReceita = true,
                        IsRecorrente = receita.IsRecurring
                    });
                }
            }

            // 2. Processar Despesas
            foreach (var despesa in despesas)
            {
                var datasOcorrencia = CalcularDatasOcorrenciaOtimizado(
                    despesa.DataInicio,
                    despesa.DataFim,
                    despesa.IsRecurring,
                    despesa.Recurrence,
                    despesa.RecurrenceCount,
                    dataInicioRelatorio,
                    dataFimRelatorio
                );

                foreach (var data in datasOcorrencia)
                {
                    listaProcessada.Add(new TransacaoProcessada
                    {
                        Data = data,
                        Valor = despesa.Valor,
                        Descricao = despesa.Titulo ?? "Despesa",
                        Categoria = despesa.CategoriaNome ?? "Sem Categoria",
                        EhReceita = false,
                        IsRecorrente = despesa.IsRecurring
                    });
                }
            }

            return listaProcessada.OrderBy(t => t.Data).ToList();
        }

        #endregion

        #region Análise de Categorias (Vilã e Heroína)

        /// <summary>
        /// Identifica a categoria com maior aumento (vilã) e maior redução (heroína) de gastos
        /// </summary>
        public static AnaliseCategoriaResult AnalisarCategorias(
            List<TransacaoProcessada> despesasAtuais,
            List<TransacaoProcessada> despesasAnteriores)
        {
            var result = new AnaliseCategoriaResult();

            // Agrupar por categoria
            var gastosAtuais = despesasAtuais
                .GroupBy(d => d.Categoria)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Valor));

            var gastosAnteriores = despesasAnteriores
                .GroupBy(d => d.Categoria)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Valor));

            // Calcular variações
            var variacoes = new List<(string Categoria, decimal Variacao, decimal ValorAtual)>();

            foreach (var categoria in gastosAtuais.Keys.Union(gastosAnteriores.Keys))
            {
                var atual = gastosAtuais.GetValueOrDefault(categoria, 0);
                var anterior = gastosAnteriores.GetValueOrDefault(categoria, 0);

                if (anterior > 0 || atual > 0)
                {
                    decimal variacao;
                    if (anterior == 0)
                        variacao = atual > 0 ? 100 : 0;
                    else
                        variacao = ((atual - anterior) / anterior) * 100;

                    variacoes.Add((categoria, variacao, atual));
                }
            }

            // Identificar Vilã (maior aumento)
            var vila = variacoes
                .Where(v => v.ValorAtual > 0)
                .OrderByDescending(v => v.Variacao)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(vila.Categoria))
            {
                result.CategoriaVila = vila.Categoria;
                result.VilaVariacao = vila.Variacao;
            }

            // Identificar Heroína (maior redução)
            var heroina = variacoes
                .Where(v => v.Variacao < 0)
                .OrderBy(v => v.Variacao)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(heroina.Categoria))
            {
                result.CategoriaHeroina = heroina.Categoria;
                result.HeroinaVariacao = heroina.Variacao;
            }

            return result;
        }

        #endregion

        #region Projeção Futura

        /// <summary>
        /// Calcula projeção de saldo para os próximos 6 meses baseada na média atual
        /// </summary>
        public static ProjecaoFuturaResult CalcularProjecaoFutura(
            List<ReceitaDTO> receitas,
            List<DespesaDTO> despesas,
            decimal saldoAtual)
        {
            var result = new ProjecaoFuturaResult();
            var hoje = DateTime.Today;

            // Calcular médias dos últimos 3 meses
            var inicioCalculo = hoje.AddMonths(-3);
            var fimCalculo = hoje;

            var transacoes3Meses = ProcessarTransacoes(receitas, despesas, inicioCalculo, fimCalculo);

            var totalMeses = 3m;
            var mediaReceitaMensal = transacoes3Meses
                .Where(t => t.EhReceita)
                .Sum(t => t.Valor) / totalMeses;

            var mediaDespesaMensal = transacoes3Meses
                .Where(t => !t.EhReceita)
                .Sum(t => t.Valor) / totalMeses;

            var mediaSaldoMensal = mediaReceitaMensal - mediaDespesaMensal;

            // Projetar 6 meses
            var saldoProjetado = saldoAtual;
            for (int i = 1; i <= 6; i++)
            {
                var mesProjetado = hoje.AddMonths(i);
                result.Meses.Add(mesProjetado.ToString("MMM/yy", CultureInfo.GetCultureInfo("pt-BR")));

                saldoProjetado += mediaSaldoMensal;
                result.Saldos.Add(Math.Round(saldoProjetado, 2));
            }

            result.MediaReceitaMensal = mediaReceitaMensal;
            result.MediaDespesaMensal = mediaDespesaMensal;
            result.PrevisaoPositiva = mediaSaldoMensal > 0;

            return result;
        }

        #endregion

        #region Cálculo de Datas Otimizado

        /// <summary>
        /// Calcula datas de ocorrência com algoritmo otimizado para repetições longas
        /// </summary>
        private static List<DateTime> CalcularDatasOcorrenciaOtimizado(
            DateTime inicioItem,
            DateTime fimItem,
            bool ehRecorrente,
            string tipoRecorrencia,
            int? qtdRepeticoes,
            DateTime filtroInicio,
            DateTime filtroFim)
        {
            var datas = new List<DateTime>();
            var dataAtual = inicioItem.Date;

            // Se NÃO é recorrente, só retorna a data se estiver dentro do período
            if (!ehRecorrente)
            {
                if (dataAtual >= filtroInicio.Date && dataAtual <= filtroFim.Date)
                {
                    datas.Add(dataAtual);
                }
                return datas;
            }

            // Configuração de limites
            int maxRepeticoes = qtdRepeticoes.HasValue && qtdRepeticoes.Value > 0 ? qtdRepeticoes.Value : 999;
            DateTime limiteSeguranca = DateTime.Today.AddYears(5);
            DateTime dataLimiteReal = fimItem != default && fimItem != DateTime.MinValue ? fimItem.Date : limiteSeguranca;
            if (dataLimiteReal > limiteSeguranca) dataLimiteReal = limiteSeguranca;

            // OTIMIZAÇÃO: Para recorrências longas, pular direto para o início do filtro
            dataAtual = AvancarParaInicioFiltro(dataAtual, filtroInicio, tipoRecorrencia);

            // Loop otimizado
            int repeticoesProcessadas = 0;
            while (dataAtual <= dataLimiteReal && dataAtual <= filtroFim.Date && repeticoesProcessadas < maxRepeticoes)
            {
                if (dataAtual >= filtroInicio.Date)
                {
                    datas.Add(dataAtual);
                }

                dataAtual = AvancarData(dataAtual, tipoRecorrencia);
                repeticoesProcessadas++;

                // Se já passou do filtro, interrompe
                if (dataAtual > filtroFim.Date) break;
            }

            return datas;
        }

        /// <summary>
        /// Avança a data inicial para o início do filtro usando cálculo matemático direto
        /// </summary>
        private static DateTime AvancarParaInicioFiltro(DateTime dataInicio, DateTime filtroInicio, string tipoRecorrencia)
        {
            if (dataInicio >= filtroInicio) return dataInicio;

            var diferenca = filtroInicio - dataInicio;

            switch (tipoRecorrencia)
            {
                case "Diario":
                case "Daily":
                    // Cálculo direto: quantos dias pular
                    return dataInicio.AddDays(diferenca.Days);

                case "Semanal":
                case "Weekly":
                    // Cálculo direto: quantas semanas pular
                    int semanas = diferenca.Days / 7;
                    var novaData = dataInicio.AddDays(semanas * 7);
                    while (novaData < filtroInicio) novaData = novaData.AddDays(7);
                    return novaData;

                case "Mensal":
                case "Monthly":
                    // Cálculo direto: quantos meses pular
                    int meses = ((filtroInicio.Year - dataInicio.Year) * 12) + (filtroInicio.Month - dataInicio.Month);
                    if (meses < 0) meses = 0;
                    var novaDataMensal = dataInicio.AddMonths(meses);
                    while (novaDataMensal < filtroInicio) novaDataMensal = novaDataMensal.AddMonths(1);
                    return novaDataMensal;

                case "Anual":
                case "Yearly":
                    // Cálculo direto: quantos anos pular
                    int anos = filtroInicio.Year - dataInicio.Year;
                    if (anos < 0) anos = 0;
                    var novaDataAnual = dataInicio.AddYears(anos);
                    while (novaDataAnual < filtroInicio) novaDataAnual = novaDataAnual.AddYears(1);
                    return novaDataAnual;

                default:
                    return dataInicio;
            }
        }

        /// <summary>
        /// Avança a data baseada no tipo de recorrência
        /// </summary>
        private static DateTime AvancarData(DateTime data, string tipoRecorrencia)
        {
            return tipoRecorrencia switch
            {
                "Diario" or "Daily" => data.AddDays(1),
                "Semanal" or "Weekly" => data.AddDays(7),
                "Mensal" or "Monthly" => data.AddMonths(1),
                "Anual" or "Yearly" => data.AddYears(1),
                _ => data.AddMonths(1) // Padrão
            };
        }

        #endregion
    }

    #region Classes de Resultado

    /// <summary>
    /// Resultado da análise de categorias vilã e heroína
    /// </summary>
    public class AnaliseCategoriaResult
    {
        public string? CategoriaVila { get; set; }
        public decimal VilaVariacao { get; set; }
        public string? CategoriaHeroina { get; set; }
        public decimal HeroinaVariacao { get; set; }
    }

    /// <summary>
    /// Resultado da projeção financeira futura
    /// </summary>
    public class ProjecaoFuturaResult
    {
        public List<string> Meses { get; set; } = new();
        public List<decimal> Saldos { get; set; } = new();
        public decimal MediaReceitaMensal { get; set; }
        public decimal MediaDespesaMensal { get; set; }
        public bool PrevisaoPositiva { get; set; }
    }

    #endregion
}