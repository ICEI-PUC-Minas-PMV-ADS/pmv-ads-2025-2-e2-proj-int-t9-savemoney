using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class ReceitasController : Controller
    {
        private readonly AppDbContext _context;

        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Receitas
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var receitas = await _context.Receitas
                .Where(r => r.UsuarioId == userId)
                .OrderByDescending(r => r.DataInicio)
                .ToListAsync();
            return View(receitas);
        }

        // GET: Receitas/Create
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateOrEditModal", new Receita());
        }

        // POST: Receitas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Valor,CurrencyType,DataInicio,DataFim,IsRecurring,Recurrence,RecurrenceCount,Recebido")] Receita receita)
        {
            // Removemos a validação do UsuarioId pois vamos setar manualmente
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                receita.UsuarioId = userId;

                _context.Add(receita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Se falhar, idealmente deveríamos retornar o erro para o modal, 
            // mas para simplificar o AJAX, redirecionamos (ou retornamos PartialView com erros)
            return RedirectToAction(nameof(Index));
        }

        // GET: Receitas/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var receita = await _context.Receitas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (receita == null) return NotFound();

            return PartialView("_CreateOrEditModal", receita);
        }

        // POST: Receitas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Valor,CurrencyType,DataInicio,DataFim,IsRecurring,Recurrence,RecurrenceCount,Recebido")] Receita receita)
        {
            if (id != receita.Id) return NotFound();

            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                    // Verifica se existe e pertence ao usuário (AsNoTracking é crucial aqui)
                    var exists = await _context.Receitas.AsNoTracking()
                        .AnyAsync(e => e.Id == id && e.UsuarioId == userId);

                    if (!exists) return NotFound();

                    receita.UsuarioId = userId;
                    _context.Update(receita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw; // Em produção tratamos melhor
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Receitas/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var receita = await _context.Receitas
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (receita != null)
            {
                _context.Receitas.Remove(receita);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}