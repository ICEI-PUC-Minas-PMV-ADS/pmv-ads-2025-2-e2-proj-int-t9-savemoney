using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using savemoney.Models;
using savemoney.Models.Enums;
using System.Security.Claims;

namespace savemoney.Controllers
{
    [Authorize]
    public class ContextoController : Controller
    {
        private readonly AppDbContext _context;

        public ContextoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> DefinirContexto([FromBody] string tipo)
        {
            if (string.IsNullOrEmpty(tipo))
                return BadRequest(new { error = "Tipo não informado" });

            var contexto = tipo.ToLower() switch
            {
                "pj" => TipoContexto.Empresarial,
                "ambos" => TipoContexto.Ambos,
                _ => TipoContexto.Pessoal
            };

            var contextoString = contexto.ToString();

            // 1. Salva em COOKIE (mais confiável que Session)
            Response.Cookies.Append("UserContext", contextoString, new CookieOptions
            {
                HttpOnly = false, // Permite JS ler se necessário
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.Now.AddYears(1),
                Path = "/"
            });

            // 2. Também salva na Session (backup)
            HttpContext.Session.SetString("UserContext", contextoString);

            // 3. Salva no banco (persistência permanente)
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var usuario = await _context.Usuarios.FindAsync(userId);
                    if (usuario != null)
                    {
                        usuario.UltimoContexto = contexto;
                        _context.Usuarios.Update(usuario);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch { }

            return Ok(new { success = true, contexto = contextoString });
        }

        [HttpGet]
        public IActionResult ObterContexto()
        {
            // Tenta ler do Cookie primeiro, depois Session
            var contexto = Request.Cookies["UserContext"]
                ?? HttpContext.Session.GetString("UserContext")
                ?? "Pessoal";

            return Ok(new { contexto });
        }
    }
}