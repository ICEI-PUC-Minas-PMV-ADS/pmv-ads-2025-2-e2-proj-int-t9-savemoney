using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using savemoney.Models;

namespace savemoney.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            var dados = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (dados == null)
            {
                ViewBag.Message = "Usuario ou senha invalidos!";
                return View();
            }

            bool senhaOk = BCrypt.Net.BCrypt.Verify(usuario.Senha, dados.Senha);

            if(senhaOk)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, dados.Id.ToString()),
                    new Claim(ClaimTypes.Name, dados.Nome),
                    new Claim(ClaimTypes.Email, dados.Email),
                    new Claim(ClaimTypes.Role, dados.TipoUsuario.ToString())
                };

                var usuarioIdentify = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(usuarioIdentify);

                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(2),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(principal, props);

                return Redirect("/");
            }
            else
            {
                ViewBag.Message = "Usuario ou senha invalidos!";
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        // GET: Usuarios/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Se não for admin, só pode ver o próprio id
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || userIdClaim != id.ToString())
                {
                    return Forbid();
                }
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,Senha,Documento,DataCadastro,Perfil,TipoUsuario")] Usuario usuario)
        {
            if (!User.IsInRole("Administrador"))
            {
                usuario.TipoUsuario = TipoUsuario.Usuario;
            }

            if (ModelState.IsValid)
            {
                usuario.DataCadastro = DateTime.Now;
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                _context.Add(usuario);
                await _context.SaveChangesAsync();

                // Adicione o claim do Id do usuário aqui
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()),
                };

                var usuarioIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(usuarioIdentity);

                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(2),
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(principal, props);
                return Redirect("/");
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Se não for admin, só pode editar o próprio usuário
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || userIdClaim != id.ToString())
                {
                    return Forbid();
                }
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Senha,Documento,DataCadastro,Perfil,TipoUsuario")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            // Se não for admin, só pode editar o próprio usuário
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || userIdClaim != id.ToString())
                {
                    return Forbid();
                }
                usuario.TipoUsuario = TipoUsuario.Usuario;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Usuarios/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Se não for admin, só pode excluir o próprio usuário
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || userIdClaim != id.ToString())
                {
                    return Forbid();
                }
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Se não for admin, só pode excluir o próprio usuário
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || userIdClaim != id.ToString())
                {
                    return Forbid();
                }
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
