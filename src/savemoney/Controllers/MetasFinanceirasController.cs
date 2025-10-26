using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace savemoney.Controllers
{
    [Authorize] // Garante que apenas usuários logados acessem este controller
    public class MetasFinanceirasController : Controller
    {
        private readonly AppDbContext _context;

        public MetasFinanceirasController(AppDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para obter o ID do usuário logado de forma segura
        private int GetUserId()
        {
            // O ClaimTypes.NameIdentifier armazena o ID do usuário após o login.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado corretamente.");
            }
            return userId;
        }

        // GET: MetasFinanceiras (Listagem)
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            // Lista apenas as metas do usuário logado
            var metas = await _context.MetasFinanceiras
                .Where(m => m.UsuarioId == userId)
                .OrderByDescending(m => m.Id)
                .ToListAsync();
            return View(metas);
        }

        // GET: MetasFinanceiras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            int userId = GetUserId();
            // Busca a meta incluindo os aportes (Eager Loading), verificando a propriedade do usuário
            var metaFinanceira = await _context.MetasFinanceiras
                .Include(m => m.Aportes)
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (metaFinanceira == null) return NotFound();

            // Ordena os aportes por data antes de enviar para a View
            if (metaFinanceira.Aportes != null)
            {
                 metaFinanceira.Aportes = metaFinanceira.Aportes.OrderByDescending(a => a.DataAporte).ToList();
            }

            return View(metaFinanceira);
        }

        // GET: MetasFinanceiras/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MetasFinanceiras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titulo,Descricao,ValorObjetivo,DataLimite")] MetaFinanceira metaFinanceira)
        {
            // Configurações automáticas
            metaFinanceira.UsuarioId = GetUserId();
            metaFinanceira.ValorAtual = 0;

            // Removemos as validações automáticas para campos que configuramos manualmente ou são relacionamentos.
            // Isso é necessário para que o ModelState.IsValid passe em versões modernas do .NET.
            ModelState.Remove("UsuarioId");
            ModelState.Remove("ValorAtual");
            ModelState.Remove("Usuario");
            ModelState.Remove("Aportes");

            if (ModelState.IsValid)
            {
                _context.Add(metaFinanceira);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(metaFinanceira);
        }

        // GET: MetasFinanceiras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            int userId = GetUserId();
            var metaFinanceira = await _context.MetasFinanceiras.FindAsync(id);

            if (metaFinanceira == null || metaFinanceira.UsuarioId != userId) return NotFound();
            
            return View(metaFinanceira);
        }

        // POST: MetasFinanceiras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,ValorObjetivo,DataLimite")] MetaFinanceira metaFinanceiraUpdate)
        {
            if (id != metaFinanceiraUpdate.Id) return NotFound();

            // Segurança: Prevenção de Overposting.
            // Busca a entidade original para garantir propriedade e impedir alteração de ValorAtual ou UsuarioId via formulário.
            int userId = GetUserId();
            // Usamos AsNoTracking para buscar a original sem que o EF tente rastreá-la.
            var metaOriginal = await _context.MetasFinanceiras.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

            if (metaOriginal == null || metaOriginal.UsuarioId != userId) return NotFound();

            // Reaplica os dados que não devem ser alterados pelo formulário
            metaFinanceiraUpdate.UsuarioId = metaOriginal.UsuarioId;
            metaFinanceiraUpdate.ValorAtual = metaOriginal.ValorAtual;

            // Remove validações desnecessárias
            ModelState.Remove("UsuarioId");
            ModelState.Remove("ValorAtual");
            ModelState.Remove("Usuario");
            ModelState.Remove("Aportes");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(metaFinanceiraUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MetaFinanceiraExists(metaFinanceiraUpdate.Id)) return NotFound();
                    else throw;
                }
                // Redireciona para os detalhes após a edição
                return RedirectToAction(nameof(Details), new { id = metaFinanceiraUpdate.Id });
            }
            return View(metaFinanceiraUpdate);
        }

        // GET: MetasFinanceiras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            int userId = GetUserId();
            var metaFinanceira = await _context.MetasFinanceiras
                .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (metaFinanceira == null) return NotFound();

            return View(metaFinanceira);
        }

        // POST: MetasFinanceiras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = GetUserId();
            var metaFinanceira = await _context.MetasFinanceiras.FindAsync(id);

            if (metaFinanceira != null && metaFinanceira.UsuarioId == userId)
            {
                // A exclusão em cascata configurada no DbContext cuidará dos Aportes.
                _context.MetasFinanceiras.Remove(metaFinanceira);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        // Ação específica para registrar aportes (chamada a partir da tela de Detalhes)
        // POST: MetasFinanceiras/RegistrarAporte
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarAporte(int MetaFinanceiraId, decimal ValorAporte)
        {
            int userId = GetUserId();
            var meta = await _context.MetasFinanceiras.FindAsync(MetaFinanceiraId);

            if (meta == null || meta.UsuarioId != userId) return NotFound();

            if (ValorAporte <= 0)
            {
                // Usamos TempData para enviar mensagens entre requisições (Padrão POST-Redirect-GET)
                TempData["ErrorMessage"] = "O valor do aporte deve ser maior que zero.";
                return RedirectToAction(nameof(Details), new { id = MetaFinanceiraId });
            }

            // 1. Cria o novo aporte
            var aporte = new Aporte
            {
                MetaFinanceiraId = MetaFinanceiraId,
                Valor = ValorAporte,
                DataAporte = DateTime.Now // Usando a data e hora atual
            };

            // 2. Atualiza o valor atual da meta
            meta.ValorAtual += ValorAporte;

            // 3. Salva ambos no banco de dados (em uma transação implícita do SaveChangesAsync)
            _context.Aportes.Add(aporte);
            _context.MetasFinanceiras.Update(meta);
            await _context.SaveChangesAsync();

            // 4. Define a mensagem apropriada (Conforme CT-009)
            if (meta.EstaConcluida)
            {
                TempData["SuccessMessage"] = "Parabéns! Você atingiu sua meta financeira!";
            }
            else
            {
                TempData["SuccessMessage"] = "Aporte registrado com sucesso!";
            }

            return RedirectToAction(nameof(Details), new { id = MetaFinanceiraId });
        }

        private bool MetaFinanceiraExists(int id)
        {
            return _context.MetasFinanceiras.Any(e => e.Id == id);
        }
    }
}