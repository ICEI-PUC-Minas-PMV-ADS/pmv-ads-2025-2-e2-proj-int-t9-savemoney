using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class DespesasController : Controller
    {
        private readonly AppDbContext _context;

        public DespesasController(AppDbContext context)
        {
            _context = context;
        }

        // Lista todas as Despesas
        public IActionResult Index()
        {
            var despesas = _context.Despesas.ToList();
            return View(despesas);
        }

        // Cria nova despesa (GET)
        public IActionResult Create()
        {            
                ViewBag.Categorias = new SelectList(_context.BudgetCategories, "Id", "Nome");
                return PartialView("_CreateOrEditModal", new Despesa());          
        }

        // Cria nova despesa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Despesa despesa)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.BudgetCategories, "Id", "Nome");
                return PartialView("_CreateOrEditModal", despesa);
            }

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Editar despesa (GET)
        public IActionResult Edit(int? id)
        {
            var despesa = _context.Despesas.Find(id);
            if (despesa == null)
                return NotFound();

            ViewBag.Categorias = new SelectList(_context.BudgetCategories, "Id", "Nome", despesa.BudgetCategoryId);
            return PartialView("_CreateOrEditModal", despesa);
        }

        // Editar despesa (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Despesa despesa)
        {
            if (id != despesa.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(_context.BudgetCategories, "Id", "Nome", despesa.BudgetCategoryId);
                return PartialView("_CreateOrEditModal", despesa);
            }

            _context.Despesas.Update(despesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Deletar despesa
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var despesa = _context.Despesas.Find(id);
            if (despesa == null)
                return NotFound();

            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
