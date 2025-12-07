using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Models.Enums;
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
                .OrderBy(c => c.TipoContabil)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        // GET: /Categories/Create
        public IActionResult Create()
        {
            return PartialView("_CreateOrEditModal", new Category { Name = "" });
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            var userId = GetCurrentUserId();
            ModelState.Remove("Usuario");

            if (ModelState.IsValid)
            {
                category.UsuarioId = userId;
                category.IsPredefined = false;

                // Sincroniza ClassificacaoDre com TipoContabil
                category.ClassificacaoDre = MapearParaClassificacaoDre(category.TipoContabil);

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

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
                    existing.TipoContabil = category.TipoContabil;

                    // Sincroniza ClassificacaoDre com TipoContabil
                    existing.ClassificacaoDre = MapearParaClassificacaoDre(category.TipoContabil);

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
                .Include(c => c.Receitas)
                .Include(c => c.Despesas)
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);

            if (category == null) return NotFound();

            // Verifica se está em uso
            if (category.BudgetCategories.Any() || category.Receitas.Any() || category.Despesas.Any())
            {
                return BadRequest("Categoria em uso. Remova as transações vinculadas primeiro.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: /Categories/ObterPorTipo - Para selects filtrados
        [HttpGet]
        public async Task<IActionResult> ObterPorTipo(TipoContabil tipo)
        {
            var userId = GetCurrentUserId();
            var categories = await _context.Categories
                .Where(c => c.UsuarioId == userId && c.TipoContabil == tipo)
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            return Json(categories);
        }

        // GET: /Categories/ObterTodas - Para selects gerais
        [HttpGet]
        public async Task<IActionResult> ObterTodas()
        {
            var userId = GetCurrentUserId();
            var categories = await _context.Categories
                .Where(c => c.UsuarioId == userId)
                .OrderBy(c => c.TipoContabil)
                .ThenBy(c => c.Name)
                .Select(c => new { c.Id, c.Name, Tipo = c.TipoContabil.ToString() })
                .ToListAsync();

            return Json(categories);
        }

        /// <summary>
        /// Mapeia TipoContabil para ClassificacaoDre (mantém compatibilidade com DRE).
        /// </summary>
        private TipoClassificacaoDre MapearParaClassificacaoDre(TipoContabil tipo)
        {
            return tipo switch
            {
                TipoContabil.CustoVariavel => TipoClassificacaoDre.CustoVariavel,
                TipoContabil.DespesaFixa => TipoClassificacaoDre.DespesaOperacional,
                TipoContabil.DespesaOperacional => TipoClassificacaoDre.DespesaOperacional,
                _ => TipoClassificacaoDre.NaoClassificado
            };
        }
    }
}