using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var categories = await _context.Categories
                .Where(c => c.UsuarioId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Categories/Create
        public IActionResult Create()
        {
            // Retorna a PartialView para o AJAX
            return PartialView("_CreateOrEditModal", new Category { Name = "" });
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            var userId = GetCurrentUserId();
            ModelState.Remove("Usuario"); // Ignora validação de navegação

            if (ModelState.IsValid)
            {
                category.UsuarioId = userId;
                category.IsPredefined = false;

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Redireciona para recarregar a lista (Simples e eficaz)
                return RedirectToAction(nameof(Index));
            }

            // Se falhar, devolve o modal com erros
            return PartialView("_CreateOrEditModal", category);
        }

        // GET: /Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var userId = GetCurrentUserId();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (category == null) return NotFound();

            return PartialView("_CreateOrEditModal", category);
        }

        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            var userId = GetCurrentUserId();

            ModelState.Remove("Usuario");

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

                    if (existing == null) return NotFound();

                    existing.Name = category.Name;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return PartialView("_CreateOrEditModal", category);
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var category = await _context.Categories
                .Include(c => c.BudgetCategories)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (category == null) return NotFound();

            if (category.BudgetCategories.Any())
            {
                return BadRequest("Categoria em uso.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}