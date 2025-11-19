// Controllers/BudgetsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services;
using System.Security.Claims;

namespace savemoney.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BudgetService _budgetService;

        public BudgetsController(AppDbContext context, BudgetService budgetService)
        {
            _context = context;
            _budgetService = budgetService;
        }

        // GET: /Budgets
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                foreach (var bc in budget.Categories)
                {
                    bc.CurrentSpent = await _budgetService.GetCurrentSpentAsync(bc.Id);
                }
            }

            return View(budgets);
        }

        // GET: /Budgets/Create
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);
            return View(new Budget { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(1) });
        }

        // POST: /Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            budget.UserId = userId;

            // VALIDAÇÃO LIMPA E SEM REPETIÇÃO
            if (budget.Categories == null || !budget.Categories.Any())
            {
                ModelState.AddModelError("", "Adicione pelo menos uma categoria ao orçamento.");
            }
            else
            {
                for (int i = 0; i < budget.Categories.Count; i++)
                {
                    var cat = budget.Categories.ElementAt(i);

                    if (cat.CategoryId <= 0)
                        ModelState.AddModelError($"Categories[{i}].CategoryId", "Selecione uma categoria válida.");

                    if (cat.Limit <= 0)
                        ModelState.AddModelError($"Categories[{i}].Limit", "O limite deve ser maior que zero.");
                }
            }

            if (!ModelState.IsValid)
            {
                // RECARREGA AS CATEGORIAS DISPONÍVEIS (essencial!)
                ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);

                // NÃO RETORNA O budget COM CATEGORIES QUEBRADAS
                // Cria um novo Budget só com os campos preenchidos pelo usuário
                return View(new Budget
                {
                    Name = budget.Name,
                    Description = budget.Description,
                    StartDate = budget.StartDate,
                    EndDate = budget.EndDate,
                    Status = budget.Status
                });
            }

            try
            {
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar o orçamento: " + ex.Message);
                ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);
                return View(new Budget
                {
                    Name = budget.Name,
                    Description = budget.Description,
                    StartDate = budget.StartDate,
                    EndDate = budget.EndDate,
                    Status = budget.Status
                });
            }
        }

        // GET: /Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var budget = await _context.Budgets
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null) return NotFound();

            ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);
            return View(budget);
        }

        // POST: /Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget, List<BudgetCategory> Categories)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            if (id != budget.Id || budget.UserId != userId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);
                return View(budget);
            }

            try
            {
                // Atualiza apenas os campos do Budget
                var existing = await _context.Budgets.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = budget.Name;
                existing.Description = budget.Description;
                existing.StartDate = budget.StartDate;
                existing.EndDate = budget.EndDate;
                existing.Status = budget.Status;

                // Atualiza BudgetCategories (sem remover tudo)
                var existingCats = await _context.BudgetCategories
                    .Where(bc => bc.BudgetId == id)
                    .ToListAsync();

                // Remove as que não estão mais na lista
                var incomingIds = Categories?.Select(c => c.Id).Where(i => i > 0).ToList() ?? new();
                var toRemove = existingCats.Where(ec => !incomingIds.Contains(ec.Id)).ToList();
                _context.BudgetCategories.RemoveRange(toRemove);

                // Atualiza ou adiciona
                foreach (var cat in Categories ?? new())
                {
                    if (cat.Id > 0)
                    {
                        var existingCat = existingCats.FirstOrDefault(ec => ec.Id == cat.Id);
                        if (existingCat != null)
                        {
                            existingCat.Limit = cat.Limit;
                        }
                    }
                    else
                    {
                        if (cat.CategoryId > 0 && cat.Limit > 0)
                        {
                            cat.BudgetId = id;
                            _context.BudgetCategories.Add(cat);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar: " + (ex.InnerException?.Message ?? ex.Message));
                ViewBag.AvailableCategories = await GetAvailableCategoriesAsync(userId);
                return View(budget);
            }
        }

        // DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // AUXILIARES
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }

       private async Task<List<dynamic>> GetAvailableCategoriesAsync(int userId)
        {
                return await _context.Categories
                    .Where(c => c.IsPredefined || c.UsuarioId == userId)
                    .OrderBy(c => c.Name)
                    .Select(c => new { id = c.Id, name = c.Name })
                    .ToListAsync<dynamic>();
        }
    }
}