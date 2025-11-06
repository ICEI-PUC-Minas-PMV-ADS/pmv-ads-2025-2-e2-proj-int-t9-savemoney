using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services; // 🔹 Importante para reconhecer o RecurrenceService

namespace savemoney.Controllers
{
    public class ReceitasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RecurrenceService _recurrenceService; // 🔹 Adicionado

        public ReceitasController(AppDbContext context, RecurrenceService recurrenceService) // 🔹 Injetado
        {
            _context = context;
            _recurrenceService = recurrenceService;
        }

        // GET: Receitas
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Receitas.Include(r => r.BudgetCategory);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Receitas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receitas
                .Include(r => r.BudgetCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // GET: Receitas/Create
        public IActionResult Create()
        {
            ViewData["BudgetCategoryId"] = new SelectList(_context.BudgetCategories, "Id", "Id");
            return View();
        }

        // POST: Receitas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Valor,Data,BudgetCategoryId,Descricao,IsRecurring,Frequency,Interval,RecurrenceEndDate,RecurrenceOccurrences")] Receita receita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();

                // 🔁 Geração automática das recorrências
                if (receita.IsRecurring && receita.Frequency != RecurrenceFrequency.None)
                {
                    var ocorrencias = _recurrenceService.GenerateOccurrences(
                        receita,
                        receita.Data,
                        receita.RecurrenceEndDate ?? receita.Data.AddMonths(12)
                    );

                    foreach (var r in ocorrencias)
                        _context.Receitas.Add(r);

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["BudgetCategoryId"] = new SelectList(_context.BudgetCategories, "Id", "Id", receita.BudgetCategoryId);
            return View(receita);
        }

        // GET: Receitas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null)
            {
                return NotFound();
            }
            ViewData["BudgetCategoryId"] = new SelectList(_context.BudgetCategories, "Id", "Id", receita.BudgetCategoryId);
            return View(receita);
        }

        // POST: Receitas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Valor,Data,BudgetCategoryId,Descricao,IsRecurring,Frequency,Interval,RecurrenceEndDate,RecurrenceOccurrences")] Receita receita)
        {
            if (id != receita.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceitaExists(receita.Id))
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
            ViewData["BudgetCategoryId"] = new SelectList(_context.BudgetCategories, "Id", "Id", receita.BudgetCategoryId);
            return View(receita);
        }

        // GET: Receitas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receitas
                .Include(r => r.BudgetCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // POST: Receitas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita != null)
            {
                _context.Receitas.Remove(receita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receitas.Any(e => e.Id == id);
        }
    }
}
