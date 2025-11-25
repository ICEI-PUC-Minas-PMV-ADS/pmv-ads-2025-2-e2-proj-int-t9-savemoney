using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    [Authorize]
    public class ThemeController : Controller
    {
        private readonly AppDbContext _context;

        public ThemeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Theme/ObterTemaAtivo
        [HttpGet]
        public async Task<IActionResult> ObterTemaAtivo()
        {
            var usuarioId = ObterUsuarioIdLogado();

            var temaAtivo = await _context.UserThemes
                .FirstOrDefaultAsync(t => t.UsuarioId == usuarioId && t.IsAtivo);

            if (temaAtivo == null)
            {
                // Retorna tema padrão (Default)
                return Json(new
                {
                    bgPrimary = "#10111a",
                    bgSecondary = "#0d0e16",
                    bgCard = "#1b1d29",
                    borderColor = "#2a2c3c",
                    textPrimary = "#f5f5ff",
                    textSecondary = "#aaaaaa",
                    accentPrimary = "#3b82f6",
                    accentPrimaryHover = "#2563eb",
                    btnPrimaryText = "#ffffff"
                });
            }

            return Json(new
            {
                id = temaAtivo.Id,
                nomeTema = temaAtivo.NomeTema,
                bgPrimary = temaAtivo.BgPrimary,
                bgSecondary = temaAtivo.BgSecondary,
                bgCard = temaAtivo.BgCard,
                borderColor = temaAtivo.BorderColor,
                textPrimary = temaAtivo.TextPrimary,
                textSecondary = temaAtivo.TextSecondary,
                accentPrimary = temaAtivo.AccentPrimary,
                accentPrimaryHover = temaAtivo.AccentPrimaryHover,
                btnPrimaryText = temaAtivo.BtnPrimaryText
            });
        }

        // GET: Theme/ListarTemas
        [HttpGet]
        public async Task<IActionResult> ListarTemas()
        {
            var usuarioId = ObterUsuarioIdLogado();

            var temas = await _context.UserThemes
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.IsAtivo)
                .ThenByDescending(t => t.DataCriacao)
                .Select(t => new
                {
                    t.Id,
                    t.NomeTema,
                    t.IsAtivo,
                    t.AccentPrimary
                })
                .ToListAsync();

            return Json(temas);
        }

        // POST: Theme/SalvarTema
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTema([FromBody] UserTheme tema)
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (tema.Id == 0)
            {
                // Novo tema
                tema.UsuarioId = usuarioId;
                tema.DataCriacao = DateTime.Now;
                tema.IsAtivo = false;

                _context.UserThemes.Add(tema);
            }
            else
            {
                // Editar tema existente
                var temaExistente = await _context.UserThemes
                    .FirstOrDefaultAsync(t => t.Id == tema.Id && t.UsuarioId == usuarioId);

                if (temaExistente == null)
                {
                    return NotFound();
                }

                temaExistente.NomeTema = tema.NomeTema;
                temaExistente.BgPrimary = tema.BgPrimary;
                temaExistente.BgSecondary = tema.BgSecondary;
                temaExistente.BgCard = tema.BgCard;
                temaExistente.BorderColor = tema.BorderColor;
                temaExistente.TextPrimary = tema.TextPrimary;
                temaExistente.TextSecondary = tema.TextSecondary;
                temaExistente.AccentPrimary = tema.AccentPrimary;
                temaExistente.AccentPrimaryHover = tema.AccentPrimaryHover;
                temaExistente.BtnPrimaryText = tema.BtnPrimaryText;

                _context.UserThemes.Update(temaExistente);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, temaId = tema.Id });
        }

        // POST: Theme/AtivarTema
        [HttpPost]
        public async Task<IActionResult> AtivarTema([FromBody] int temaId)
        {
            var usuarioId = ObterUsuarioIdLogado();

            // Desativar todos os temas do usuário
            var temas = await _context.UserThemes
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var t in temas)
            {
                t.IsAtivo = false;
            }

            // Ativar o tema selecionado
            var temaSelecionado = temas.FirstOrDefault(t => t.Id == temaId);

            if (temaSelecionado != null)
            {
                temaSelecionado.IsAtivo = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Theme/AplicarTemaPadrao
        [HttpPost]
        public async Task<IActionResult> AplicarTemaPadrao([FromBody] string nomeTema)
        {
            var usuarioId = ObterUsuarioIdLogado();

            // Desativar todos os temas customizados
            var temas = await _context.UserThemes
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var t in temas)
            {
                t.IsAtivo = false;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // DELETE: Theme/DeletarTema
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarTema(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();

            var tema = await _context.UserThemes
                .FirstOrDefaultAsync(t => t.Id == id && t.UsuarioId == usuarioId);

            if (tema == null)
            {
                return NotFound();
            }

            _context.UserThemes.Remove(tema);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // Método auxiliar
        private int ObterUsuarioIdLogado()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return 0;
        }
    }
}