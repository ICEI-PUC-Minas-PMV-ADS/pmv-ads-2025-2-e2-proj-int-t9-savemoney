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
        [HttpGet]
        public async Task<IActionResult> ObterRecentes()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            var userId = int.Parse(userIdString);

            // 1. Executa as verificações do R11 em tempo real
            await _servico.SincronizarSistema(userId);
            await _servico.VerificarAlertasOrcamento(userId);
            await _servico.VerificarContasProximas(userId);

            // 2. Busca as notificações do banco
            var notificacoes = await _context.Notificacoes
                .AsNoTracking()
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.DataCriacao)
                .Take(100)
                .Select(n => new
                {
                    n.Id,
                    n.Titulo,
                    n.Mensagem,
                    n.Lida,
                    n.LinkAcao,
                    Data = n.DataCriacao.ToString("dd/MM HH:mm"),
                    Tipo = n.Tipo.ToString()
                })
                .ToListAsync();

            // 3. Conta quantas não foram lidas
            var naoLidas = notificacoes.Count(n => !n.Lida);

            return Json(new { notificacoes, naoLidas });
        }

        // POST: /Notificacoes/MarcarComoLida/5
        [HttpPost]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            var userId = int.Parse(userIdString);

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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            var userId = int.Parse(userIdString);

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
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            var userId = int.Parse(userIdString);

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