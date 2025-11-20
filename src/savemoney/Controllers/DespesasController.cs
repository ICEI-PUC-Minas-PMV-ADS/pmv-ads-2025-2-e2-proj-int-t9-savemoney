using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using savemoney.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore; 

public class DespesasController : Controller

{

    private readonly AppDbContext _context;

    public DespesasController(AppDbContext context)

    {

        _context = context;

    }

    public IActionResult Index()

    {

        var despesas = _context.Despesas

            .Include(d => d.BudgetCategory)

                .ThenInclude(bc => bc!.Category)

            .ToList();

        return View(despesas);

    }

    // GET: Create

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

        if (!ModelState.IsValid)

        {

            CarregarBudgetCategoriesDropdown(despesa.BudgetCategoryId);

            return PartialView("_CreateOrEditModal", despesa);

        }

        _context.Despesas.Add(despesa);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    // GET: Edit

    public IActionResult Edit(int? id)

    {

        var despesa = _context.Despesas.Find(id);

        if (despesa == null) return NotFound();

        CarregarBudgetCategoriesDropdown(despesa.BudgetCategoryId);

        return PartialView("_EditModal", despesa);

    }

    // POST: Edit

    [HttpPost]

    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Edit(int id, Despesa despesa)

    {

        if (id != despesa.Id) return NotFound();

        if (!ModelState.IsValid)

        {

            CarregarBudgetCategoriesDropdown(despesa.BudgetCategoryId);

            return PartialView("_EditModal", despesa);

        }

        _context.Update(despesa);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));

    }

    [HttpPost]

    public async Task<IActionResult> Delete(int id)

    {

        var despesa = await _context.Despesas.FindAsync(id);

        if (despesa == null) return NotFound();

        _context.Despesas.Remove(despesa);

        await _context.SaveChangesAsync();

        return Ok();

    }

    // MÉTODO MÁGICO: Carrega dropdown com limite, gasto e cor

    private void CarregarBudgetCategoriesDropdown(int? selectedId = null)

    {

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");

        var orcamentoAtivo = _context.Budgets

            .Where(b => b.UserId == userId &&

                        b.StartDate <= DateTime.Today &&

                        b.EndDate >= DateTime.Today)

            .FirstOrDefault();

        var itens = new List<SelectListItem>

        {

            new() { Value = "", Text = "– Sem orçamento (não controla limite) –" }

        };

        if (orcamentoAtivo != null)

        {

            var categorias = _context.BudgetCategories

                .Where(bc => bc.BudgetId == orcamentoAtivo.Id)

                .Include(bc => bc.Category)

                .Select(bc => new SelectListItem

                {

                    Value = bc.Id.ToString(),

                    Text = bc.Category.Name +

                           $" (limite: {bc.Limit:C} | usado: {bc.CurrentSpent:C})" +

                           (bc.CurrentSpent >= bc.Limit ? " ESTOURADO!" :

                            bc.CurrentSpent >= bc.Limit * 0.9m ? " QUASE!" :

                            bc.CurrentSpent >= bc.Limit * 0.7m ? " CUIDADO" : ""),

                    Selected = bc.Id == selectedId

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

                Text = "Nenhum orçamento ativo no momento",

                Disabled = true

            });

        }

        ViewBag.BudgetCategories = itens;

    }

}
