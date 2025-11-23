using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Services
{
    public class BudgetService
    {
        private readonly AppDbContext _context;

        public BudgetService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calcula o total gasto em uma BudgetCategory (baseado em Despesas REAIS)
        /// </summary>
        /// <param name="budgetCategoryId">ID da BudgetCategory</param>
        /// <returns>Total gasto (decimal)</returns>
        public async Task<decimal> GetCurrentSpentAsync(int budgetCategoryId)
        {
            // Agora que a tabela Despesas existe, fazemos a soma real
            var total = await _context.Despesas
                .Where(d => d.BudgetCategoryId == budgetCategoryId)
                .SumAsync(d => (decimal?)d.Valor) ?? 0m;

            return total;
        }

        /// <summary>
        /// Verifica se uma nova despesa pode ser adicionada sem estourar o limite
        /// </summary>
        public async Task<bool> CanAddExpenseAsync(int budgetCategoryId, decimal amount)
        {
            if (amount <= 0) return false;

            var budgetCategory = await _context.BudgetCategories
                .FirstOrDefaultAsync(bc => bc.Id == budgetCategoryId);

            if (budgetCategory == null) return false;

            var currentSpent = await GetCurrentSpentAsync(budgetCategoryId);
            return (currentSpent + amount) <= budgetCategory.Limit;
        }

        /// <summary>
        /// Retorna o progresso de uma categoria (0 a 100+)
        /// </summary>
        public async Task<decimal> GetProgressPercentageAsync(int budgetCategoryId)
        {
            var budgetCategory = await _context.BudgetCategories
                .FirstOrDefaultAsync(bc => bc.Id == budgetCategoryId);

            if (budgetCategory == null || budgetCategory.Limit <= 0) return 0;

            var spent = await GetCurrentSpentAsync(budgetCategoryId);
            var percentage = (spent / budgetCategory.Limit) * 100m;

            return percentage; // Removi o Math.Min para permitir ver estouros (>100%)
        }

        /// <summary>
        /// Retorna status visual da categoria (success, warning, danger)
        /// </summary>
        public async Task<string> GetProgressStatusAsync(int budgetCategoryId)
        {
            var percentage = await GetProgressPercentageAsync(budgetCategoryId);
            return percentage switch
            {
                <= 80 => "success",
                <= 100 => "warning",
                _ => "danger"
            };
        }
    }
}