using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class BudgetsController : Controller
    {
        private readonly AppDbContext _context;

        public BudgetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Budgets
        public async Task<IActionResult> Index()
        {
            var budgets = await _context.Budgets
                .Include(b => b.Usuario)
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return View(budgets);
        }

        // GET: Budgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Usuario)
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // GET: Budgets/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento");
            ViewBag.Categories = _context.Categories
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();

            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Budget budget, List<BudgetCategory> Categories)
        {
            // REATIVADO: Validação
            if (ModelState.IsValid)
            {
                try
                {
                    // Valida UserId
                    if (!_context.Usuarios.Any(u => u.Id == budget.UserId))
                    {
                        ModelState.AddModelError("UserId", "Usuário inválido.");
                        return RecarregarView(budget);
                    }

                    _context.Budgets.Add(budget);
                    await _context.SaveChangesAsync();

                    if (Categories != null && Categories.Any())
                    {
                        foreach (var cat in Categories)
                        {
                            if (cat.CategoryId <= 0 || cat.Limit <= 0)
                                throw new InvalidOperationException("Categoria ou limite inválido.");

                            cat.BudgetId = budget.Id;
                        }
                        _context.BudgetCategories.AddRange(Categories);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao salvar: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            return RecarregarView(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Categories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (budget == null) return NotFound();

            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();

            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget, List<BudgetCategory> Categories)
        {
            if (id != budget.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Remove antigas
                    var existing = await _context.BudgetCategories
                        .Where(bc => bc.BudgetId == budget.Id)
                        .ToListAsync();
                    _context.BudgetCategories.RemoveRange(existing);

                    // Atualiza Budget
                    _context.Update(budget);
                    await _context.SaveChangesAsync();

                    // Adiciona novas
                    if (Categories != null && Categories.Any())
                    {
                        foreach (var cat in Categories)
                        {
                            cat.BudgetId = budget.Id;
                        }
                        _context.BudgetCategories.AddRange(Categories);
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.Id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();

            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var budget = await _context.Budgets
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (budget == null) return NotFound();

            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var budget = await _context.Budgets.FindAsync(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetExists(int id) => _context.Budgets.Any(e => e.Id == id);

        // MÉTODO AUXILIAR
        private IActionResult RecarregarView(Budget budget)
        {
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();
            return View("Create", budget);
        }
    }
}