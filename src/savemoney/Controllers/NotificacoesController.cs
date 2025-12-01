using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;
using savemoney.Services;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class NotificacoesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ServicoNotificacao _servico;

        public NotificacoesController(AppDbContext context, ServicoNotificacao servico)
        {
            _context = context;
            _servico = servico;
        }

        // GET: /Notificacoes/ObterRecentes
        // Chamado via AJAX pelo sininho no Header
        [HttpGet]
        public async Task<IActionResult> ObterRecentes()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            // 1. Executa as verificações do R11 em tempo real
            // Isso garante que se o usuário acabou de estourar um orçamento, o alerta apareça agora
            await _servico.SincronizarSistema(userId);        // Updates do JSON
            await _servico.VerificarAlertasOrcamento(userId); // R11: Orçamento
            await _servico.VerificarContasProximas(userId);   // R11: Contas

            // 2. Busca as notificações do banco
            var notificacoes = await _context.Notificacoes
                .AsNoTracking()
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.DataCriacao)
                .Take(100) // Limite visual (o serviço já limpa o banco, mas garantimos aqui)
                .Select(n => new
                {
                    n.Id,
                    n.Titulo,
                    n.Mensagem,
                    n.Lida,
                    n.LinkAcao, // Link para redirecionamento (ex: ir para orçamento)
                    Data = n.DataCriacao.ToString("dd/MM HH:mm"),
                    Tipo = n.Tipo.ToString() // "Sucesso", "Erro", "AlertaOrcamento", etc.
                })
                .ToListAsync();

            // 3. Conta quantas não foram lidas para a bolinha vermelha (Badge)
            var naoLidas = notificacoes.Count(n => !n.Lida);

            return Json(new { notificacoes, naoLidas });
        }

        // POST: /Notificacoes/MarcarComoLida/5
        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var notif = await _context.Notificacoes
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);

            if (notif != null && !notif.Lida)
            {
                notif.Lida = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // POST: /Notificacoes/MarcarTodasComoLidas
        [HttpPost]
        public async Task<IActionResult> MarcarTodasComoLidas()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var naoLidas = await _context.Notificacoes
                .Where(n => n.UsuarioId == userId && !n.Lida)
                .ToListAsync();

            if (naoLidas.Any())
            {
                foreach (var n in naoLidas) n.Lida = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // POST: /Notificacoes/Excluir/5
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var notif = await _context.Notificacoes
                .FirstOrDefaultAsync(n => n.Id == id && n.UsuarioId == userId);

            if (notif != null)
            {
                _context.Notificacoes.Remove(notif);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}