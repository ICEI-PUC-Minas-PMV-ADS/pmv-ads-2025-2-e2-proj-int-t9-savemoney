using System;
using System.Collections.Generic;
using System.Linq;
using savemoney.Models;

namespace savemoney.Services.Helpers
{
    /// <summary>
    /// Classe Helper responsável pelos cálculos estatísticos de tendências financeiras
    /// 
    /// SOLID - SRP (Single Responsibility Principle):
    /// Esta classe tem UMA ÚNICA responsabilidade: calcular tendências
    /// Não conhece nada sobre banco de dados, controllers ou views
    /// </summary>
    public class CalculadoraTendencia
    {
        /// <summary>
        /// Identifica o tipo de tendência com base na variação percentual total
        /// </summary>
        /// <param name="variacaoPercentual">Variação entre primeiro e último mês</param>
        /// <returns>Tipo de tendência identificada</returns>
        public TipoTendencia IdentificarTendencia(double variacaoPercentual)
        {
            // Margem de 5% para considerar como estável (evita oscilações pequenas)
            const double margemEstabilidade = 5.0;

            if (variacaoPercentual > margemEstabilidade)
                return TipoTendencia.Crescente;

            if (variacaoPercentual < -margemEstabilidade)
                return TipoTendencia.Decrescente;

            return TipoTendencia.Estavel;
        }

        /// <summary>
        /// Calcula a variação percentual entre dois valores
        /// </summary>
        /// <param name="valorAnterior">Valor inicial</param>
        /// <param name="valorAtual">Valor final</param>
        /// <returns>Percentual de variação (ex: 15.5 para crescimento de 15,5%)</returns>
        public double CalcularVariacaoPercentual(double valorAnterior, double valorAtual)
        {
            if (valorAnterior == 0)
                return 0; // Evita divisão por zero

            return ((valorAtual - valorAnterior) / Math.Abs(valorAnterior)) * 100;
        }

        /// <summary>
        /// Calcula a média móvel dos últimos N períodos
        /// Útil para suavizar variações bruscas e identificar tendências reais
        /// 
        /// FEATURE: Opção B - Média Móvel
        /// </summary>
        /// <param name="valores">Lista de valores (saldos mensais)</param>
        /// <param name="janela">Tamanho da janela (padrão: 3 meses)</param>
        /// <returns>Média móvel calculada</returns>
        public double CalcularMediaMovel(List<double> valores, int janela = 3)
        {
            if (valores == null || valores.Count == 0)
                return 0;

            // Se houver menos valores que a janela, usa todos disponíveis
            var valoresParaCalculo = valores.Count >= janela
                ? valores.TakeLast(janela).ToList()
                : valores;

            return valoresParaCalculo.Average();
        }

        /// <summary>
        /// Identifica outliers (valores atípicos) usando o método IQR (Interquartile Range)
        /// Um valor é considerado outlier se estiver 1.5x além do intervalo interquartil
        /// 
        /// FEATURE: Opção B - Detecção de Outliers
        /// </summary>
        /// <param name="valores">Lista de valores para análise</param>
        /// <returns>Lista de índices dos outliers identificados</returns>
        public List<int> IdentificarOutliers(List<double> valores)
        {
            var outliers = new List<int>();

            if (valores == null || valores.Count < 4) // Mínimo de 4 valores para IQR confiável
                return outliers;

            var ordenados = valores.OrderBy(v => v).ToList();

            // Calcula Q1 (25º percentil) e Q3 (75º percentil)
            int n = ordenados.Count;
            double q1 = ordenados[n / 4];
            double q3 = ordenados[(3 * n) / 4];
            double iqr = q3 - q1;

            // Limites para identificação de outliers
            double limiteInferior = q1 - (1.5 * iqr);
            double limiteSuperior = q3 + (1.5 * iqr);

            // Identifica índices dos outliers
            for (int i = 0; i < valores.Count; i++)
            {
                if (valores[i] < limiteInferior || valores[i] > limiteSuperior)
                {
                    outliers.Add(i);
                }
            }

            return outliers;
        }

        /// <summary>
        /// Gera uma mensagem descritiva sobre a tendência identificada
        /// </summary>
        /// <param name="tendencia">Tipo de tendência</param>
        /// <param name="variacao">Percentual de variação</param>
        /// <returns>Mensagem formatada para exibição ao usuário</returns>
        public string GerarMensagemTendencia(TipoTendencia tendencia, double variacao)
        {
            var variacaoFormatada = Math.Abs(variacao).ToString("F1");

            return tendencia switch
            {
                TipoTendencia.Crescente =>
                    $"📈 Suas finanças apresentam uma tendência crescente de {variacaoFormatada}%. Continue assim!",

                TipoTendencia.Decrescente =>
                    $"📉 Suas finanças apresentam uma tendência decrescente de {variacaoFormatada}%. Atenção aos gastos!",

                TipoTendencia.Estavel =>
                    $"➡️ Suas finanças estão estáveis, com variação de apenas {variacaoFormatada}%.",

                TipoTendencia.Indefinida =>
                    "❓ Não há dados suficientes para identificar uma tendência confiável. Adicione mais transações.",

                _ => "Análise não disponível."
            };
        }

        /// <summary>
        /// TODO: Feature futura - Identificar sazonalidade nos dados
        /// Detectar padrões que se repetem em períodos específicos (ex: gastos maiores em dezembro)
        /// </summary>
        /// <param name="dadosMensais">Dados mensais para análise</param>
        /// <returns>Informações sobre padrões sazonais identificados</returns>
        public string IdentificarSazonalidade(List<DadosMensalViewModel> dadosMensais)
        {
            // TODO: Implementação futura
            // - Analisar se há meses com padrões recorrentes
            // - Ex: "Dezembro sempre apresenta gastos 30% acima da média"
            // - Requer histórico de pelo menos 12 meses para ser confiável

            return "Análise de sazonalidade estará disponível em versões futuras.";
        }
    }
}