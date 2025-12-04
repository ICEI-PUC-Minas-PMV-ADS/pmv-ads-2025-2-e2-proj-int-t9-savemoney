using savemoney.Models.ViewModels;

namespace savemoney.services.Interfaces
{
    /// <summary>
    /// Interface para o servico de KPIs Corporativos.
    /// </summary>
    public interface IKpisCorporativosService
    {
        /// <summary>
        /// Calcula todos os KPIs para o periodo especificado.
        /// </summary>
        /// <param name="usuarioId">ID do usuario autenticado.</param>
        /// <param name="dataInicio">Data inicial do periodo.</param>
        /// <param name="dataFim">Data final do periodo.</param>
        /// <returns>ViewModel com todos os KPIs calculados.</returns>
        Task<KpisCorporativosViewModel> CalcularKpisAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim);

        /// <summary>
        /// Obtem o historico mensal para graficos.
        /// </summary>
        /// <param name="usuarioId">ID do usuario.</param>
        /// <param name="meses">Quantidade de meses (padrao: 6).</param>
        /// <returns>Lista de dados mensais.</returns>
        Task<List<KpiMensal>> ObterHistoricoMensalAsync(
            int usuarioId,
            int meses = 6);

        /// <summary>
        /// Obtem custos agrupados por categoria.
        /// </summary>
        /// <param name="usuarioId">ID do usuario.</param>
        /// <param name="dataInicio">Data inicial.</param>
        /// <param name="dataFim">Data final.</param>
        /// <returns>Lista de custos por categoria.</returns>
        Task<List<CustoCategoria>> ObterCustosPorCategoriaAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim);
    }
}