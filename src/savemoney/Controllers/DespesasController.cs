using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class DespesasController : Controller
    {
        private readonly AppDbContext _context;

        public DespesasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Despesas
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var despesas = await _context.Despesas
                .Where(d => d.UsuarioId == userId)
                .Include(d => d.BudgetCategory)
                    .ThenInclude(bc => bc!.Category)
                .OrderByDescending(d => d.DataInicio) // Ordenar por data (mais recente primeiro)
                .ToListAsync();

            return View(despesas);
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            CarregarBudgetCategoriesDropdown();
            return PartialView("_CreateOrEditModal", new Despesa());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Despesa despesa)
        {
            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");
            // Removemos BudgetCategory da validação pois é preenchido via ID
            ModelState.Remove("BudgetCategory");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Dados inválidos. Verifique os campos.";
                return RedirectToAction(nameof(Index));
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return BadRequest("Usuário não autenticado");

            despesa.UsuarioId = int.Parse(userIdClaim);

            _context.Despesas.Add(despesa);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Despesa registrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var despesa = await _context.Despesas
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == userId);

            if (despesa == null) return NotFound();

            CarregarBudgetCategoriesDropdown(despesa.BudgetCategoryId);

            // PADRONIZAÇÃO: Usamos o mesmo modal de Create
            return PartialView("_CreateOrEditModal", despesa);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Despesa despesa)
        {
            if (id != despesa.Id) return NotFound();

            ModelState.Remove("UsuarioId");
            ModelState.Remove("Usuario");
            ModelState.Remove("BudgetCategory");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // AsNoTracking para evitar conflito de tracking do EF Core
            var despesaExistente = await _context.Despesas
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == userId);

            if (despesaExistente == null) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Dados inválidos na edição.";
                return RedirectToAction(nameof(Index));
            }

            despesa.UsuarioId = userId;
            _context.Update(despesa);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Despesa atualizada!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var despesa = await _context.Despesas
                .FirstOrDefaultAsync(d => d.Id == id && d.UsuarioId == userId);

            if (despesa == null) return NotFound();

            _context.Despesas.Remove(despesa);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // MÉTODO AUXILIAR: Carrega dropdown com limite, gasto e cor
        private void CarregarBudgetCategoriesDropdown(int? selectedId = null)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return;

            var userId = int.Parse(userIdStr);

            var orcamentoAtivo = _context.Budgets
                .Where(b => b.UserId == userId &&
                            b.StartDate <= DateTime.Today &&
                            b.EndDate >= DateTime.Today)
                .FirstOrDefault();

            var itens = new List<SelectListItem>
            {
                new() { Value = "", Text = "– Sem vínculo (Geral) –" }
            };

            if (orcamentoAtivo != null)
            {
                var categorias = _context.BudgetCategories
                    .Where(bc => bc.BudgetId == orcamentoAtivo.Id)
                    .Include(bc => bc.Category)
                    .Select(bc => new
                    {
                        bc.Id,
                        CategoryName = bc.Category.Name,
                        bc.Limit,
                        bc.CurrentSpent
                    })
                    .ToList() // Traz para memória para formatar string complexa
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = $"{x.CategoryName} (Restam: {(x.Limit - x.CurrentSpent):C})",
                        Selected = x.Id == selectedId
                    })
                    .OrderBy(i => i.Text)
                    .ToList();

                itens.AddRange(categorias);
            }
            else
            {
                itens.Add(new SelectListItem
                {
                    Value = "",
                    Text = "Nenhum orçamento ativo no momento (Crie um em Estratégia)",
                    Disabled = true
                });
            }

            ViewBag.BudgetCategories = itens;
        }
    }
}