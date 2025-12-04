using savemoney.Models.Enums;
using savemoney.Models.ViewModels;

namespace savemoney.services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de cálculo do DRE Gerencial.
    /// </summary>
    public interface IDreGerencialService
    {
        /// <summary>
        /// Gera o DRE completo para o período e regime especificados.
        /// </summary>
        /// <param name="usuarioId">ID do usuário autenticado.</param>
        /// <param name="dataInicio">Data inicial do período.</param>
        /// <param name="dataFim">Data final do período.</param>
        /// <param name="regime">Regime de apuração (Competência ou Caixa).</param>
        /// <returns>ViewModel com dados do DRE calculados.</returns>
        Task<DreGerencialViewModel> GerarDreAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim,
            TipoRegimeDre regime);

        /// <summary>
        /// Gera o DRE com comparativo entre dois períodos.
        /// </summary>
        Task<DreGerencialViewModel> GerarDreComparativoAsync(
            int usuarioId,
            DateTime dataInicio,
            DateTime dataFim,
            DateTime dataInicioComparativo,
            DateTime dataFimComparativo,
            TipoRegimeDre regime);

        /// <summary>
        /// Obtém o histórico mensal para gráficos de evolução.
        /// </summary>
        /// <param name="usuarioId">ID do usuário autenticado.</param>
        /// <param name="meses">Quantidade de meses para buscar (padrão: 12).</param>
        /// <param name="regime">Regime de apuração.</param>
        /// <returns>Lista de dados mensais.</returns>
        Task<List<DreMensal>> ObterHistoricoMensalAsync(
            int usuarioId,
            int meses = 12,
            TipoRegimeDre regime = TipoRegimeDre.Competencia);
    }
}