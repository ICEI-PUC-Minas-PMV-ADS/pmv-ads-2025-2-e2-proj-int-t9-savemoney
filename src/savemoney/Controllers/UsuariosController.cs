using System;
using System.Collections.Generic;
using System.IO;
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

            if (senhaOk)
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

                return RedirectToAction("Index", "Dashboard");
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
            return Redirect("/");
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
                return RedirectToAction("Index", "Dashboard");
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

        // POST: Usuarios/UploadFotoPerfil
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFotoPerfil(IFormFile foto)
        {
            try
            {
                // Obter ID do usuário logado
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Validar se arquivo foi enviado
                if (foto == null || foto.Length == 0)
                {
                    return Json(new { success = false, message = "Nenhuma foto foi enviada." });
                }

                // Validar tamanho (2MB)
                if (foto.Length > 2 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "A foto deve ter no máximo 2MB." });
                }

                // Validar tipo de arquivo
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensao))
                {
                    return Json(new { success = false, message = "Apenas imagens são permitidas (JPG, PNG, GIF, WEBP)." });
                }

                // Criar pasta se não existir
                var pastaUploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(pastaUploads))
                {
                    Directory.CreateDirectory(pastaUploads);
                }

                // Gerar nome único para o arquivo
                var nomeArquivo = $"{usuarioId}_{DateTime.Now:yyyyMMddHHmmss}{extensao}";
                var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

                // Deletar foto antiga se existir (para economizar espaço)
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario != null && !string.IsNullOrEmpty(usuario.FotoPerfil) &&
                    usuario.FotoPerfil.StartsWith("/uploads/"))
                {
                    var caminhoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                        usuario.FotoPerfil.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                    if (System.IO.File.Exists(caminhoAntigo))
                    {
                        System.IO.File.Delete(caminhoAntigo);
                    }
                }

                // Salvar arquivo
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                // Atualizar caminho no banco
                var caminhoRelativo = $"/uploads/avatars/{nomeArquivo}";

                if (usuario != null)
                {
                    usuario.FotoPerfil = caminhoRelativo;
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }

                return Json(new
                {
                    success = true,
                    caminhoFoto = caminhoRelativo,
                    message = "Foto atualizada com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Erro ao fazer upload: {ex.Message}"
                });
            }
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