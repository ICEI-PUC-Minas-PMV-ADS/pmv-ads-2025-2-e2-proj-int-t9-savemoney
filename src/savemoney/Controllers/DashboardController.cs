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
                .Where(w => w.UsuarioId == usuarioId)
                .OrderBy(w => w.PosicaoY)
                .ThenBy(w => w.PosicaoX)
                .ToListAsync();

            ViewBag.Usuario = usuario;
            ViewBag.Widgets = widgets;

            return View();
        }

        // POST: Dashboard/SalvarWidget
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarWidget([Bind("Id,Titulo,Descricao,ImagemUrl,Link,Colunas,Largura,CorFundo")] Widget widget)
        {
            var usuarioId = ObterUsuarioIdLogado();

            if (widget.Id == 0)
            {
                // Novo widget
                widget.UsuarioId = usuarioId;
                widget.DataCriacao = DateTime.Now;

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
}