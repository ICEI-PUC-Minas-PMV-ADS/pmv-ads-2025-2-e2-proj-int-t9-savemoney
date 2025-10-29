using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System.Linq;

namespace savemoney.Controllers
{
    public class ReceitasController : Controller
    {
        private readonly AppDbContext _context;
        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        // Get Recietas

     
        public async Task<IActionResult> Index()
        {
            var receitas = await _context.Set<Receita>().ToListAsync();
            return View(receitas);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var receita = await _context.Set<Receita>().FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null) return NotFound();

            return View(receita);   
        }

     
        public IActionResult Create()
        {
            return View();
        }
        //Post Receita/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Titulo, Valor, Data, Categoria, Descricao")] Receita receita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(receita);
        }

        //Edit Receita
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null) return NotFound();
            var receita = await _context.Set<Receita>().FindAsync(id);
            if (receita == null) return NotFound();
            return View(receita);
        }

        //Post EditReceita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Titulo, Valor, Data, Categoria, Descricao")] Receita receita)
        {
            if (id != receita.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Set<Receita>().Any(e => e.Id == receita.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(receita);
        }

        //Get Receita/delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var receita = await _context.Set<Receita>().FirstOrDefaultAsync(m => m.Id == id);
            if (receita == null) return NotFound();
            return View(receita);
        }

        //Post Receita/Delete
        [HttpPost, ActionName ("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Set<Receita>().FindAsync(id);
            if (receita != null) _context.Set<Receita>().Remove(receita);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}