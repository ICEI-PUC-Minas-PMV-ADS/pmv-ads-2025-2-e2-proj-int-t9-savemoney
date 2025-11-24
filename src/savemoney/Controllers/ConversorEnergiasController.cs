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
        // Tarifa Média Nacional para simulações (R$/kWh) - Usado como exemplo
        private const double TarifaPadraoKWh = 0.75;

        // Listas estáticas para os Dropdowns
        private static readonly List<string> TipoValorOptions = new() { "kWh", "Reais" };
        private static readonly List<string> EstadosBrasil = new() { "AC", "SP", "RJ", "MG", "BA", "RS", "PR", "SC", "DF", "Outros" };
        private static readonly List<string> Modalidades = new() { "Rede Convencional", "Solar On-Grid", "Solar Off-Grid", "Outro" };
        private static readonly List<string> Dispositivos = new() { "Geladeira", "Ar-condicionado", "Computador", "Chuveiro Elétrico", "Lâmpada LED", "Outro" };
        private static readonly List<string> Bandeiras = new() { "Verde", "Amarela", "Vermelha 1", "Vermelha 2" };

        public ConversorEnergiasController(AppDbContext context)
        {
            _context = context;
        }

        // Helper para carregar as ViewBags
        private void CarregarViewBags()
        {
            ViewBag.TipoValorOptions = new SelectList(TipoValorOptions);
            ViewBag.EstadosBrasil = new SelectList(EstadosBrasil);
            ViewBag.Modalidades = new SelectList(Modalidades);
            ViewBag.Dispositivos = new SelectList(Dispositivos);
            ViewBag.Bandeiras = new SelectList(Bandeiras);
        }

        // Helper para gerar dicas personalizadas
        private string GerarDicaPersonalizada(ConversorEnergia conversor)
        {
            var consumo = conversor.ConsumoMensal ?? 0;
            var custoEstimado = consumo * TarifaPadraoKWh;

            if (consumo > 500 && conversor.Modalidade.Contains("Rede Convencional"))
            {
                // CORREÇÃO: A tag de imagem foi isolada para não quebrar a string C#.
                return $"Seu consumo mensal é alto ({consumo:N0} kWh). Você poderia economizar até {custoEstimado * 0.7:C} mensais ao instalar energia solar on-grid. ";
            }
            if (consumo > 200 && conversor.TipoDispositivo.Contains("Chuveiro Elétrico"))
            {
                return "O Chuveiro Elétrico é um dos maiores vilões. Reduzir 5 minutos de banho pode diminuir seu consumo em 15 kWh/mês!";
            }
            return "Monitore seus equipamentos em standby. Pequenos vazamentos de energia somam no final do mês. Use a Projeção Financeira!";
        }

        // GET: ConversorEnergias
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConversoresEnergia.ToListAsync());
        }

        // GET: ConversorEnergias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var conversorEnergia = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(m => m.Id == id);

            if (conversorEnergia == null) return NotFound();

            ViewBag.Dica = GerarDicaPersonalizada(conversorEnergia);
            ViewBag.Tarifa = TarifaPadraoKWh.ToString("C");

            return View(conversorEnergia);
        }

        // GET: ConversorEnergias/Create (Modal)
        public IActionResult Create()
        {
            CarregarViewBags();
            return PartialView("_ConversorModal", new ConversorEnergia());
        }

        // GET: ConversorEnergias/Edit/5 (Modal)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var conversorEnergia = await _context.ConversoresEnergia.FindAsync(id);
            if (conversorEnergia == null) return NotFound();

            CarregarViewBags();
            return PartialView("_ConversorModal", conversorEnergia);
        }

        // POST: Unificado para Create e Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(ConversorEnergia conversorEnergia)
        {
            CarregarViewBags();

            if (ModelState.IsValid)
            {
                // 1. Lógica de Conversão (Define ConsumoMensal em kWh)
                // Usamos a Tarifa Padrão aqui.
                double valorTarifa = TarifaPadraoKWh;

                if (conversorEnergia.TipoValor.Equals("Reais", StringComparison.OrdinalIgnoreCase))
                {
                    // R$ -> kWh
                    conversorEnergia.ConsumoMensal = conversorEnergia.ValorBase / valorTarifa;
                }
                else if (conversorEnergia.TipoValor.Equals("kWh", StringComparison.OrdinalIgnoreCase))
                {
                    // kWh -> kWh (Apenas transferimos ValorBase para ConsumoMensal para consistência)
                    conversorEnergia.ConsumoMensal = conversorEnergia.ValorBase;
                }

                // 2. Persistência
                if (conversorEnergia.Id == 0)
                {
                    _context.Add(conversorEnergia); // Create
                }
                else
                {
                    // Busca a entidade original e apenas atualiza os campos
                    var existing = await _context.ConversoresEnergia.AsNoTracking().FirstOrDefaultAsync(c => c.Id == conversorEnergia.Id);
                    if (existing == null) return NotFound();

                    _context.Update(conversorEnergia); // Edit
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se falhar na validação, retorna o modal com erros
            return PartialView("_ConversorModal", conversorEnergia);
        }

        // GET: ConversorEnergias/Delete/5 (Modal de Confirmação)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var conversorEnergia = await _context.ConversoresEnergia.FindAsync(id);
            if (conversorEnergia == null) return NotFound();

            return PartialView("_DeleteModal", conversorEnergia);
        }

        // POST: ConversorEnergias/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
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