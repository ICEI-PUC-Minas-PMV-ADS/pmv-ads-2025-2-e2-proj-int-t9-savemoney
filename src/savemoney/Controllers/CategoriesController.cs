// Controllers/CategoryController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
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

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var userCategories = await _context.Categories
                .Where(c => c.UsuarioId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(userCategories);
        }

        // GET: /Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Category category)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            category.UsuarioId = userId;
            category.IsPredefined = false; // Força

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Categoria '{category.Name}' criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            if (id == null) return NotFound();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            if (id != category.Id) return NotFound();

            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (existing == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            existing.Name = category.Name;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categoria atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var category = await _context.Categories
                .Include(c => c.BudgetCategories)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (category == null) return NotFound();

            if (category.BudgetCategories.Any())
            {
                TempData["Error"] = "Não é possível excluir: a categoria está em uso em um orçamento.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Categoria excluída com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}