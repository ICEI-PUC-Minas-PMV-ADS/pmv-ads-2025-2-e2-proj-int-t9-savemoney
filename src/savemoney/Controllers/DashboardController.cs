using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var usuarioId = ObterUsuarioIdLogado();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            var widgets = await _context.Widgets
                .Where(w => w.UsuarioId == usuarioId && w.IsVisivel)
                .OrderBy(w => w.ZIndex)
                .ThenBy(w => w.PosicaoY)
                .ThenBy(w => w.PosicaoX)
                .ToListAsync();

            ViewBag.Usuario = usuario;
            ViewBag.Widgets = widgets;

            return View();
        }

        // POST: Dashboard/SalvarWidget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarWidget([Bind("Id,Titulo,Descricao,ImagemUrl,Link,Colunas,Largura,CorFundo,TipoWidget")] Widget widget)
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (widget.Id == 0)
            {
                // Novo widget
                widget.UsuarioId = usuarioId;
                widget.DataCriacao = DateTime.Now;
                widget.IsVisivel = true;
                widget.IsPinned = false;
                widget.ZIndex = 0;

                // Calcular próxima posição disponível
                var ultimoWidget = await _context.Widgets
                    .Where(w => w.UsuarioId == usuarioId)
                    .OrderByDescending(w => w.PosicaoY)
                    .ThenByDescending(w => w.PosicaoX)
                    .FirstOrDefaultAsync();

                if (ultimoWidget != null)
                {
                    widget.PosicaoX = ultimoWidget.PosicaoX + ultimoWidget.Colunas;
                    widget.PosicaoY = ultimoWidget.PosicaoY;

                    // Se ultrapassar 3 colunas, vai para próxima linha
                    if (widget.PosicaoX >= 3)
                    {
                        widget.PosicaoX = 0;
                        widget.PosicaoY = ultimoWidget.PosicaoY + 1;
                    }
                }
                else
                {
                    widget.PosicaoX = 0;
                    widget.PosicaoY = 0;
                }

                _context.Widgets.Add(widget);
            }
            else
            {
                // Atualizar widget existente
                var widgetExistente = await _context.Widgets
                    .FirstOrDefaultAsync(w => w.Id == widget.Id && w.UsuarioId == usuarioId);

                if (widgetExistente == null)
                {
                    return NotFound();
                }

                widgetExistente.Titulo = widget.Titulo;
                widgetExistente.Descricao = widget.Descricao;
                widgetExistente.ImagemUrl = widget.ImagemUrl;
                widgetExistente.Link = widget.Link;
                widgetExistente.Colunas = widget.Colunas;
                widgetExistente.Largura = widget.Largura;
                widgetExistente.CorFundo = widget.CorFundo;
                widgetExistente.TipoWidget = widget.TipoWidget;

                _context.Widgets.Update(widgetExistente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Dashboard/ObterWidget/5
        [HttpGet]
        public async Task<IActionResult> ObterWidget(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();

            var widget = await _context.Widgets
                .FirstOrDefaultAsync(w => w.Id == id && w.UsuarioId == usuarioId);

            if (widget == null)
            {
                return NotFound();
            }

            return Json(widget);
        }

        // POST: Dashboard/DeletarWidget/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletarWidget(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();

            var widget = await _context.Widgets
                .FirstOrDefaultAsync(w => w.Id == id && w.UsuarioId == usuarioId);

            if (widget == null)
            {
                return NotFound();
            }

            _context.Widgets.Remove(widget);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Dashboard/AtualizarPosicoes
        [HttpPost]
        public async Task<IActionResult> AtualizarPosicoes([FromBody] List<WidgetPosicaoDto> posicoes)
        {
            var usuarioId = ObterUsuarioIdLogado();

            foreach (var pos in posicoes)
            {
                var widget = await _context.Widgets
                    .FirstOrDefaultAsync(w => w.Id == pos.Id && w.UsuarioId == usuarioId);

                if (widget != null && !widget.IsPinned)
                {
                    widget.PosicaoX = pos.X;
                    widget.PosicaoY = pos.Y;
                    widget.ZIndex = pos.ZIndex;
                    widget.UltimaMovimentacao = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // POST: Dashboard/ToggleVisibilidade
        [HttpPost]
        public async Task<IActionResult> ToggleVisibilidade(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var widget = await _context.Widgets.FirstOrDefaultAsync(w => w.Id == id && w.UsuarioId == usuarioId);

            if (widget == null) return NotFound();

            widget.IsVisivel = !widget.IsVisivel;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isVisivel = widget.IsVisivel });
        }

        // POST: Dashboard/FixarWidget
        [HttpPost]
        public async Task<IActionResult> FixarWidget(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var widget = await _context.Widgets.FirstOrDefaultAsync(w => w.Id == id && w.UsuarioId == usuarioId);

            if (widget == null) return NotFound();

            widget.IsPinned = !widget.IsPinned;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isPinned = widget.IsPinned });
        }

        // Método auxiliar para obter o ID do usuário logado
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

    // DTO para transferência de posições
    public class WidgetPosicaoDto
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ZIndex { get; set; }
    }
}