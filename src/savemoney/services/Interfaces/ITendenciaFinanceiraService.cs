using System.Threading.Tasks;
using savemoney.Models;

namespace savemoney.Services.Interfaces
{
    /// <summary>
    /// Interface do serviço de análise de tendências financeiras
    /// 
    /// SOLID - DIP (Dependency Inversion Principle):
    /// O Controller dependerá desta INTERFACE, não da implementação concreta
    /// Isso permite trocar a implementação sem alterar o Controller
    /// 
    /// SOLID - ISP (Interface Segregation Principle):
    /// Interface pequena e focada, contendo apenas o essencial
    /// </summary>
    public interface ITendenciaFinanceiraService
    {
        /// <summary>
        /// Analisa as tendências financeiras de um usuário em um período específico
        /// </summary>
        /// <param name="usuarioId">ID do usuário autenticado</param>
        /// <param name="meses">Quantidade de meses a serem analisados (1 a 12)</param>
        /// <returns>Relatório completo da análise de tendências</returns>
        Task<RelatorioTendenciaViewModel> AnalisarTendenciasPorPeriodoAsync(int usuarioId, int meses);
    }
}