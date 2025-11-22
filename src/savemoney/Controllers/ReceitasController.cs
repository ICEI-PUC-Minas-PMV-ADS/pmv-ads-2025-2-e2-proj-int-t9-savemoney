using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
    public class ReceitasController : Controller
    {
        private readonly AppDbContext _context;

        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        // Lista todas as receitas
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var receita = _context.Receitas
                .Where(r => r.UsuarioId == userId)
                .ToList();

            return View(receita);
        }

        // Cria nova receita (GET)
        public IActionResult Create()
        {
            return PartialView("_CreateOrEditModal", new Receita());
        }

        // Cria nova receita (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Receita receita)
        {
            // Remover validação de UsuarioId pois será preenchido manualmente
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
                return PartialView("_CreateOrEditModal", receita);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Usuário não autenticado");
            }

            receita.UsuarioId = int.Parse(userIdClaim);
            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Editar receita (GET)
        public IActionResult Edit(int? id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var receita = _context.Receitas
                .FirstOrDefault(r => r.Id == id && r.UsuarioId == userId);

            if (receita == null)
                return NotFound();

            return PartialView("_EditModal", receita);
        }

        // Editar receita (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Receita receita)
        {
            if (id != receita.Id)
                return NotFound();

            // Remover validação de UsuarioId pois será preenchido manualmente
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Usuário não autenticado");
            }

            var userId = int.Parse(userIdClaim);
            var receitaExistente = await _context.Receitas
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (receitaExistente == null)
                return NotFound();

            if (!ModelState.IsValid)
                return PartialView("_EditModal", receita);

            receita.UsuarioId = userId;
            _context.Receitas.Update(receita);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Deletar receita
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var receita = await _context.Receitas
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == userId);

            if (receita == null) return NotFound();

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}