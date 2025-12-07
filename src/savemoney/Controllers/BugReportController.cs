using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using savemoney.Models;
using System.Text.Json;

namespace savemoney.Controllers
{
    [Authorize]
    public class BugReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _jsonPath;

        public BugReportController(IWebHostEnvironment env)
        {
            _env = env;
            _jsonPath = Path.Combine(_env.ContentRootPath, "Data", "bug-reports.json");

            // Garantir que o diretório existe
            var dir = Path.GetDirectoryName(_jsonPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);
        }

        // POST: /BugReport/Enviar (qualquer usuário logado pode enviar)
        // Nota: [Authorize] já protege este endpoint
        [HttpPost]
        public async Task<IActionResult> Enviar([FromBody] BugReportDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Titulo) || string.IsNullOrWhiteSpace(dto.Descricao))
            {
                return Json(new { success = false, message = "Dados inválidos. Título e descrição são obrigatórios." });
            }

            try
            {
                var reports = await CarregarReports();

                var novoId = reports.Count > 0 ? reports.Max(r => r.Id) + 1 : 1;

                var bugReport = new BugReport
                {
                    Id = novoId,
                    Titulo = dto.Titulo.Trim(),
                    Pagina = dto.Pagina?.Trim() ?? "",
                    Descricao = dto.Descricao.Trim(),
                    Gravidade = dto.Gravidade ?? "media",
                    UserAgent = dto.UserAgent ?? "",
                    UsuarioNome = User.Identity?.Name,
                    UsuarioId = GetUserId(),
                    DataCriacao = DateTime.Now,
                    Resolvido = false
                };

                reports.Add(bugReport);
                await SalvarReports(reports);

                return Json(new { success = true, message = "Bug reportado com sucesso!", id = novoId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar bug report: {ex.Message}");
                return Json(new { success = false, message = "Erro interno ao salvar o report. Tente novamente." });
            }
        }

        // GET: /BugReport/Listar (APENAS ADMIN)
        public async Task<IActionResult> Listar()
        {
            if (!IsAdmin())
                return RedirectToAction("Index", "Dashboard");

            var reports = await CarregarReports();
            return View(reports.OrderByDescending(r => r.DataCriacao).ToList());
        }

        // GET: /BugReport/Json (APENAS ADMIN - para debug/export)
        public async Task<IActionResult> Json()
        {
            if (!IsAdmin())
                return Forbid();

            var reports = await CarregarReports();
            return new JsonResult(reports.OrderByDescending(r => r.DataCriacao).ToList(),
                new JsonSerializerOptions { WriteIndented = true });
        }

        // POST: /BugReport/MarcarResolvido (APENAS ADMIN)
        [HttpPost]
        public async Task<IActionResult> MarcarResolvido(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Acesso negado" });

            var reports = await CarregarReports();
            var report = reports.FirstOrDefault(r => r.Id == id);

            if (report != null)
            {
                report.Resolvido = true;
                await SalvarReports(reports);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Report não encontrado" });
        }

        // POST: /BugReport/Excluir (APENAS ADMIN)
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Acesso negado" });

            var reports = await CarregarReports();
            var report = reports.FirstOrDefault(r => r.Id == id);

            if (report != null)
            {
                reports.Remove(report);
                await SalvarReports(reports);
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Report não encontrado" });
        }

        // ============ HELPERS ============

        /// <summary>
        /// Verifica se o usuário atual é Administrador
        /// Baseado no TipoUsuario enum que é salvo como Role no login
        /// </summary>
        private bool IsAdmin()
        {
            // Verifica pela Role (TipoUsuario.ToString() é salvo como Role no login)
            if (User.IsInRole("Administrador"))
                return true;

            // Fallback: verificar por email específico
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(email) && email.Equals("admin@savemoney.com", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private async Task<List<BugReport>> CarregarReports()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<BugReport>();

            try
            {
                var json = await System.IO.File.ReadAllTextAsync(_jsonPath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<BugReport>();

                return JsonSerializer.Deserialize<List<BugReport>>(json) ?? new List<BugReport>();
            }
            catch
            {
                return new List<BugReport>();
            }
        }

        private async Task SalvarReports(List<BugReport> reports)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(reports, options);
            await System.IO.File.WriteAllTextAsync(_jsonPath, json);
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
        }
    }

    // DTO para receber do JavaScript
    public class BugReportDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Pagina { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string? Gravidade { get; set; }
        public string? UserAgent { get; set; }
    }
}