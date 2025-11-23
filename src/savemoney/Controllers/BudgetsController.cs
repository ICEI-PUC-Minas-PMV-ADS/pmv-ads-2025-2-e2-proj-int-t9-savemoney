using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class BudgetsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BudgetService _budgetService;

        public BudgetsController(AppDbContext context, BudgetService budgetService)
        {
            _context = context;
            _budgetService = budgetService;
        }

        // GET: /Budgets (Lista)
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Usuarios");

            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId)
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            // Calcula o progresso para exibir na lista (Barras de progresso)
            foreach (var budget in budgets)
            {
                foreach (var bc in budget.Categories)
                {
                    bc.CurrentSpent = await _budgetService.GetCurrentSpentAsync(bc.Id);
                }
            }

            return View(budgets);
        }

        // GET: /Budgets/Create (Modal)
        public async Task<IActionResult> Create()
        {
            var userId = GetCurrentUserId();
            ViewBag.AvailableCategories = await GetAvailableCategoriesListAsync(userId);

            // Inicializa com datas padrão (Hoje até +30 dias)
            return PartialView("_CreateOrEditModal", new Budget
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Status = BudgetStatus.Ativo
            });
        }

        // POST: /Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget)
        {
            var userId = GetCurrentUserId();
            budget.UserId = userId;

            // Remove validações automáticas que não fazem sentido no contexto
            ModelState.Remove("Usuario");
            ModelState.Remove("Categories");

            if (ModelState.IsValid)
            {
                _context.Budgets.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se falhar, recarrega o modal
            ViewBag.AvailableCategories = await GetAvailableCategoriesListAsync(userId);
            return PartialView("_CreateOrEditModal", budget);
        }

        // GET: /Budgets/Edit/5 (Modal)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();

            var budget = await _context.Budgets
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null) return NotFound();

            ViewBag.AvailableCategories = await GetAvailableCategoriesListAsync(userId);
            return PartialView("_CreateOrEditModal", budget);
        }

        // POST: /Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget)
        {
            if (id != budget.Id) return NotFound();
            var userId = GetCurrentUserId();

            ModelState.Remove("Usuario");
            ModelState.Remove("Categories");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBudget = await _context.Budgets
                        .Include(b => b.Categories)
                        .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

                    if (existingBudget == null) return NotFound();

                    // Atualiza dados básicos
                    existingBudget.Name = budget.Name;
                    existingBudget.Description = budget.Description;
                    existingBudget.StartDate = budget.StartDate;
                    existingBudget.EndDate = budget.EndDate;
                    existingBudget.Status = budget.Status;

                    // Lógica de Atualização de Categorias
                    var incomingCategoryIds = budget.Categories
                        .Where(c => c.CategoryId > 0)
                        .Select(c => c.CategoryId)
                        .ToList();

                    // Remover as que não estão mais na lista
                    var toRemove = existingBudget.Categories
                        .Where(c => !incomingCategoryIds.Contains(c.CategoryId))
                        .ToList();

                    _context.BudgetCategories.RemoveRange(toRemove);

                    // Adicionar ou Atualizar
                    foreach (var cat in budget.Categories)
                    {
                        if (cat.CategoryId <= 0) continue;

                        var existingCat = existingBudget.Categories
                            .FirstOrDefault(c => c.CategoryId == cat.CategoryId);

                        if (existingCat != null)
                        {
                            existingCat.Limit = cat.Limit;
                        }
                        else
                        {
                            existingBudget.Categories.Add(new BudgetCategory
                            {
                                CategoryId = cat.CategoryId,
                                Limit = cat.Limit,
                                BudgetId = id
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            ViewBag.AvailableCategories = await GetAvailableCategoriesListAsync(userId);
            return PartialView("_CreateOrEditModal", budget);
        }

        // GET: /Budgets/Details/5 (AGORA COMO MODAL)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();

            var budget = await _context.Budgets
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null) return NotFound();

            // Calcula gastos reais para o Dashboard do Modal
            foreach (var bc in budget.Categories)
            {
                bc.CurrentSpent = await _budgetService.GetCurrentSpentAsync(bc.Id);
            }

            // ALTERADO: Retorna PartialView para ser renderizado dentro do modal
            return PartialView("_DetailsModal", budget);
        }

        // POST: /Budgets/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // GET: /Budgets/ExportPdf/5
        public async Task<IActionResult> ExportPdf(int id)
        {
            var userId = GetCurrentUserId();
            var budget = await _context.Budgets
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null) return NotFound();

            foreach (var bc in budget.Categories)
            {
                bc.CurrentSpent = await _budgetService.GetCurrentSpentAsync(bc.Id);
            }

            var pdfGenerator = new BudgetPdfGenerator();
            var pdfBytes = pdfGenerator.GeneratePdf(budget);
            var fileName = $"Orcamento_{budget.Name}_{DateTime.Now:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        // Helpers
        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }

        private async Task<List<SelectListItem>> GetAvailableCategoriesListAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.IsPredefined || c.UsuarioId == userId)
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }
    }
}