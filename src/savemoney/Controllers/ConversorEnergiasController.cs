using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class ConversorEnergiasController : Controller
    {
        private readonly AppDbContext _context;

        public ConversorEnergiasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ConversorEnergias
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConversoresEnergia.ToListAsync());
        }

        // GET: ConversorEnergias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversorEnergia = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversorEnergia == null)
            {
                return NotFound();
            }

            return View(conversorEnergia);
        }

        private static readonly List<string> TipoValorOptions = new() { "Watts", "Reais" };
        private static readonly List<string> EstadosBrasil = new()
        {
            "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG","PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
        };
        private static readonly List<string> Dispositivos = new()
        {
            "Geladeira", "Ar-condicionado", "Televisão", "Micro-ondas", "Computador", "Chuveiro Elétrico",
            "Máquina de Lavar", "Lâmpada", "Ventilador", "Forno Elétrico",
            "Casa", "Carro", "Patinete", "Moto"
        };

        // GET: ConversorEnergias/Create
        public IActionResult Create()
        {
            ViewBag.TipoValorOptions = new SelectList(TipoValorOptions);
            ViewBag.EstadosBrasil = new SelectList(EstadosBrasil);
            ViewBag.Dispositivos = new SelectList(Dispositivos);
            return View();
        }

        // POST: ConversorEnergias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ValorBase,TipoValor,Estado,Modalidade,BandeiraTarifaria,TipoDispositivo,TempoUso,ConsumoMensal")] ConversorEnergia conversorEnergia)
        {
            ViewBag.TipoValorOptions = new SelectList(TipoValorOptions);
            ViewBag.EstadosBrasil = new SelectList(EstadosBrasil);
            ViewBag.Dispositivos = new SelectList(Dispositivos);

            if (ModelState.IsValid)
            {
                // Conversion logic
                if (conversorEnergia.TipoValor.Equals("Reais", StringComparison.OrdinalIgnoreCase))
                {
                    // Example: 1 Real = 0.5 kWh (replace with real formula)
                    conversorEnergia.ConsumoMensal = conversorEnergia.ValorBase * 0.5;
                }
                else if (conversorEnergia.TipoValor.Equals("Watts", StringComparison.OrdinalIgnoreCase))
                {
                    // Example: 1 kWh = 2 Reais (replace with real formula)
                    conversorEnergia.ConsumoMensal = conversorEnergia.ValorBase / 2;
                }
                // Add more conversion logic as needed

                _context.Add(conversorEnergia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(conversorEnergia);
        }

        // GET: ConversorEnergias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversorEnergia = await _context.ConversoresEnergia.FindAsync(id);
            if (conversorEnergia == null)
            {
                return NotFound();
            }
            return View(conversorEnergia);
        }

        // POST: ConversorEnergias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ValorBase,TipoValor,Estado,Modalidade,BandeiraTarifaria,TipoDispositivo,TempoUso,ConsumoMensal")] ConversorEnergia conversorEnergia)
        {
            if (id != conversorEnergia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conversorEnergia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConversorEnergiaExists(conversorEnergia.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(conversorEnergia);
        }

        // GET: ConversorEnergias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var conversorEnergia = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(m => m.Id == id);
            if (conversorEnergia == null)
            {
                return NotFound();
            }

            return View(conversorEnergia);
        }

        // POST: ConversorEnergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var conversorEnergia = await _context.ConversoresEnergia.FindAsync(id);
            if (conversorEnergia != null)
            {
                _context.ConversoresEnergia.Remove(conversorEnergia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConversorEnergiaExists(int id)
        {
            return _context.ConversoresEnergia.Any(e => e.Id == id);
        }
    }
}
