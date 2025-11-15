using Microsoft.AspNetCore.Mvc;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class ReceitasController : Controller
    {
        private readonly AppDbContext _context;

        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        // Lista todas as receita
        public IActionResult Index()
        {
            var receita = _context.Receitas.ToList();
            return View(receita);
        }

        // Cria nova receita (GET)
        public IActionResult Create()
        {
            return PartialView("_CreateOrEditModal", new Receita());
        }

        // Cria nova resceita(POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Receita receita)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateOrEditModal", receita);

            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Editar receita (GET)
        public IActionResult Edit(int? id)
        {
            var receita = _context.Receitas.Find(id);
            if (receita == null)
                return NotFound();

            return PartialView("_CreateOrEditModal", receita);
        }

        // Editar receita (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Receita receita)
        {
            if (id != receita.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return PartialView("_CreateOrEditModal", receita);

            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Deletar receita
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var receita = _context.Receitas.Find(id);
            if (receita == null)
                return NotFound();

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
