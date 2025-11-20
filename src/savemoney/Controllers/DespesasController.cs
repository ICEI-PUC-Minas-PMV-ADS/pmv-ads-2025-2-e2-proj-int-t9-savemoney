using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private void CarregarCategorias()
{
    ViewBag.Categorias = new SelectList(
        _context.BudgetCategories
            .Include(b => b.Category)   // <---- OBRIGATÓRIO
            .ToList(),
        "Id",                          // value
        "Category.Name"                // texto exibido
    );
}

        public IActionResult Index()
        {
            var despesas = _context.Despesas.ToList();
            return View(despesas);
        }

        // ----------- CREATE (GET) -----------
        public IActionResult Create()
        {
            ViewBag.Categorias = new SelectList(
                _context.BudgetCategories
                    .Include(b => b.Category),
                "Id",
                "Category.Nome"
            );

            return PartialView("_CreateOrEditModal", new Despesa());
        }

        // ----------- CREATE (POST) -----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Despesa despesa)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(
                    _context.BudgetCategories
                        .Include(b => b.Category),
                    "Id",
                    "Category.Nome",
                    despesa.BudgetCategoryId
                );

                return PartialView("_CreateOrEditModal", despesa);
            }

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ----------- EDIT (GET) -----------
        public IActionResult Edit(int? id)
        {
            var despesa = _context.Despesas.Find(id);
            if (despesa == null)
                return NotFound();

            ViewBag.Categorias = new SelectList(
                _context.BudgetCategories
                    .Include(b => b.Category),
                "Id",
                "Category.Nome",
                despesa.BudgetCategoryId
            );

            return PartialView("_CreateOrEditModal", despesa);
        }

        // ----------- EDIT (POST) -----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Despesa despesa)
        {
            if (id != despesa.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = new SelectList(
                    _context.BudgetCategories
                        .Include(b => b.Category),
                    "Id",
                    "Category.Nome",
                    despesa.BudgetCategoryId
                );

                return PartialView("_CreateOrEditModal", despesa);
            }

            _context.Despesas.Update(despesa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ----------- DELETE -----------
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var despesa = _context.Despesas.Find(id);
            if (despesa == null)
                return NotFound();

            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
