using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    [Authorize] // ✅ NOVO: Requer autenticação
    public class ConversorEnergiasController : Controller
    {
        private readonly AppDbContext _context;

        // Tarifas médias por estado (R$/kWh) - Valores aproximados de 2024
        private static readonly Dictionary<string, double> TarifasPorEstado = new()
        {
            { "AC", 0.85 }, { "SP", 0.75 }, { "RJ", 0.82 }, { "MG", 0.78 },
            { "BA", 0.70 }, { "RS", 0.72 }, { "PR", 0.68 }, { "SC", 0.71 },
            { "DF", 0.76 }, { "Outros", 0.75 }
        };

        // Adicional por bandeira tarifária (valor adicional sobre a tarifa base)
        private static readonly Dictionary<string, double> AdicionalBandeira = new()
        {
            { "Verde", 0.0 },
            { "Amarela", 0.02 },
            { "Vermelha 1", 0.04 },
            { "Vermelha 2", 0.07 }
        };

        // Listas para os Dropdowns
        private static readonly List<string> TipoValorOptions = new() { "Watts", "Reais" };
        private static readonly List<string> EstadosBrasil = new() { "AC", "SP", "RJ", "MG", "BA", "RS", "PR", "SC", "DF", "Outros" };
        private static readonly List<string> Modalidades = new() { "Rede Convencional", "Solar On-Grid", "Solar Off-Grid", "Outro" };
        private static readonly List<string> Dispositivos = new()
        {
            "Geladeira", "Ar-condicionado", "Computador",
            "Chuveiro Elétrico", "Lâmpada LED", "TV", "Micro-ondas", "Outro"
        };
        private static readonly List<string> Bandeiras = new() { "Verde", "Amarela", "Vermelha 1", "Vermelha 2" };

        public ConversorEnergiasController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ NOVO: Método para obter UsuarioId
        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            return int.Parse(userIdClaim);
        }

        private void CarregarViewBags()
        {
            ViewBag.TipoValorOptions = new SelectList(TipoValorOptions);
            ViewBag.EstadosBrasil = new SelectList(EstadosBrasil);
            ViewBag.Modalidades = new SelectList(Modalidades);
            ViewBag.Dispositivos = new SelectList(Dispositivos);
            ViewBag.Bandeiras = new SelectList(Bandeiras);
        }

        // Calcula a tarifa final considerando estado e bandeira
        private double ObterTarifaFinal(string estado, string bandeira)
        {
            double tarifaBase = TarifasPorEstado.ContainsKey(estado)
                ? TarifasPorEstado[estado]
                : 0.75;

            double adicional = AdicionalBandeira.ContainsKey(bandeira)
                ? AdicionalBandeira[bandeira]
                : 0.0;

            return tarifaBase + adicional;
        }

        // Gera dica personalizada
        private string GerarDicaPersonalizada(ConversorEnergia conversor)
        {
            var consumo = conversor.ConsumoMensal ?? 0;
            var custo = conversor.CustoMensal ?? 0;

            if (consumo > 500 && conversor.Modalidade.Contains("Rede Convencional"))
            {
                var economiaEstimada = custo * 0.70; // 70% de economia com solar
                return $"Seu consumo mensal é alto ({consumo:F2} kWh). Você poderia economizar até {economiaEstimada:C} mensais ao instalar energia solar on-grid!";
            }

            if (consumo > 200 && conversor.TipoDispositivo.Contains("Chuveiro"))
            {
                return "O Chuveiro Elétrico é um dos maiores vilões do consumo. Reduzir 5 minutos de banho pode diminuir seu consumo em até 15 kWh/mês!";
            }

            if (consumo > 100 && conversor.TipoDispositivo.Contains("Ar-condicionado"))
            {
                return "Ar-condicionado consome muito! Mantenha a temperatura em 23-24°C e faça manutenção regular dos filtros para economizar até 30%.";
            }

            if (conversor.BandeiraTarifaria == "Vermelha 1" || conversor.BandeiraTarifaria == "Vermelha 2")
            {
                return "Bandeira tarifária alta! Reduza o consumo em horários de pico (18h-21h) para economizar mais.";
            }

            return "Monitore equipamentos em standby. Pequenos vazamentos de energia somam no final do mês. Use tomadas com interruptor!";
        }

        // ✅ ATUALIZADO: Filtra por usuário
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterUsuarioId();
            var conversores = await _context.ConversoresEnergia
                .Where(c => c.UsuarioId == usuarioId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();

            return View(conversores);
        }

        // ✅ ATUALIZADO: Valida propriedade do usuário
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var usuarioId = ObterUsuarioId();
            var conversor = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == usuarioId);

            if (conversor == null) return NotFound();

            var tarifaFinal = ObterTarifaFinal(conversor.Estado, conversor.BandeiraTarifaria);
            ViewBag.Dica = GerarDicaPersonalizada(conversor);
            ViewBag.Tarifa = tarifaFinal.ToString("C");

            return View(conversor);
        }

        public IActionResult Create()
        {
            CarregarViewBags();
            return PartialView("_ConversorModal", new ConversorEnergia());
        }

        // ✅ ATUALIZADO: Valida propriedade do usuário
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuarioId = ObterUsuarioId();
            var conversor = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conversor == null) return NotFound();

            CarregarViewBags();
            return PartialView("_ConversorModal", conversor);
        }

        // ✅ ATUALIZADO: Adiciona UsuarioId ao criar/editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit(ConversorEnergia conversor)
        {
            ModelState.Remove("Usuario");
            ModelState.Remove("UsuarioId");

            CarregarViewBags();

            if (ModelState.IsValid)
            {
                var usuarioId = ObterUsuarioId();

                // ✅ Se for criação, define o UsuarioId
                if (conversor.Id == 0)
                {
                    conversor.UsuarioId = usuarioId;
                }
                else
                {
                    // ✅ Se for edição, valida que pertence ao usuário
                    var conversorExistente = await _context.ConversoresEnergia
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == conversor.Id && c.UsuarioId == usuarioId);

                    if (conversorExistente == null)
                    {
                        return Forbid();
                    }

                    conversor.UsuarioId = usuarioId;
                }

                // Obtém tarifa final considerando estado e bandeira
                double tarifaFinal = ObterTarifaFinal(conversor.Estado, conversor.BandeiraTarifaria);

                // LÓGICA CORRIGIDA DE CONVERSÃO
                if (conversor.TipoValor.Equals("Watts", StringComparison.OrdinalIgnoreCase))
                {
                    // ENTRADA: Potência em Watts
                    // CÁLCULO: Consumo = (Potência em kW) × Horas/dia × 30 dias
                    double potenciaKW = conversor.ValorBase / 1000.0; // Converte Watts para kW
                    conversor.ConsumoMensal = potenciaKW * conversor.TempoUso * 30;
                    conversor.CustoMensal = conversor.ConsumoMensal * tarifaFinal;
                }
                else if (conversor.TipoValor.Equals("Reais", StringComparison.OrdinalIgnoreCase))
                {
                    // ENTRADA: Custo mensal em R$
                    // CÁLCULO: Consumo = Custo / Tarifa
                    conversor.CustoMensal = conversor.ValorBase;
                    conversor.ConsumoMensal = conversor.ValorBase / tarifaFinal;
                }

                // Arredonda para 2 casas decimais
                conversor.ConsumoMensal = Math.Round(conversor.ConsumoMensal ?? 0, 2);
                conversor.CustoMensal = Math.Round(conversor.CustoMensal ?? 0, 2);

                // Persistência
                if (conversor.Id == 0)
                {
                    _context.Add(conversor);
                }
                else
                {
                    _context.Update(conversor);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Conversão salva com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return PartialView("_ConversorModal", conversor);
        }

        // ✅ ATUALIZADO: Valida propriedade do usuário
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuarioId = ObterUsuarioId();
            var conversor = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conversor == null) return NotFound();

            return PartialView("_DeleteModal", conversor);
        }

        // ✅ ATUALIZADO: Valida propriedade do usuário
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioId = ObterUsuarioId();
            var conversor = await _context.ConversoresEnergia
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

            if (conversor != null)
            {
                _context.ConversoresEnergia.Remove(conversor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Conversão excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}