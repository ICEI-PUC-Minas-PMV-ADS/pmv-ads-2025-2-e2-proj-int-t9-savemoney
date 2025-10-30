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
            var appDbContext = _context.Budgets.Include(b => b.Usuario);
            return View(await appDbContext.ToListAsync());
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
            ViewBag.Categories = _context.Categories.ToList(); // ← ESSENCIAL PARA O DROPDOWN
            return View();
        }

        // POST: Budgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Budget budget,
            List<BudgetCategory> Categories) // ← RECEBE A LISTA DE CATEGORIAS
        {
            if (ModelState.IsValid)
            {
                // Atribui as categorias ao orçamento
                budget.Categories = Categories ?? new List<BudgetCategory>();

                _context.Add(budget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se der erro, recarrega as listas
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories.ToList();
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets
                .Include(b => b.Categories) // ← CARREGA AS CATEGORIAS JÁ SALVAS
                .FirstOrDefaultAsync(b => b.Id == id);

            if (budget == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories.ToList();
            return View(budget);
        }

        // POST: Budgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Budget budget, List<BudgetCategory> Categories)
        {
            if (id != budget.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Remove categorias antigas
                    var existingCategories = _context.BudgetCategories
                        .Where(bc => bc.BudgetId == budget.Id);
                    _context.BudgetCategories.RemoveRange(existingCategories);

                    // Adiciona as novas
                    budget.Categories = Categories ?? new List<BudgetCategory>();
                    foreach (var cat in budget.Categories)
                    {
                        cat.BudgetId = budget.Id;
                    }

                    _context.Update(budget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BudgetExists(budget.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Documento", budget.UserId);
            ViewBag.Categories = _context.Categories.ToList();
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var budget = await _context.Budgets
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (budget == null)
            {
                return NotFound();
            }

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
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BudgetExists(int id)
        {
            return _context.Budgets.Any(e => e.Id == id);
        }
    }
}
