using System;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using savemoney.Models;
using savemoney.Services;

namespace savemoney.Pages.Relatorios
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public RelatorioViewModel ViewModel { get; set; } = new RelatorioViewModel();

        [BindProperty]
        public RelatorioRequest Request { get; set; } = new RelatorioRequest();

        [BindProperty]
        public string? ChartImageBase64 { get; set; }

        [BindProperty]
        public string? PieImageBase64 { get; set; }

        public IndexModel(AppDbContext context) => _context = context;

        public void OnGet()
        {
            Request = new RelatorioRequest { StartDate = DateTime.Today.AddMonths(-3), EndDate = DateTime.Today, Period = AggregationPeriod.Monthly };
            var userId = GetUserId();
            ViewModel = ReportService.BuildReport(_context, userId, Request.StartDate, Request.EndDate, Request.Period);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            var userId = GetUserId();
            ViewModel = ReportService.BuildReport(_context, userId, Request.StartDate, Request.EndDate, Request.Period);
            return Page();
        }

        public IActionResult OnPostExportExcel()
        {
            var userId = GetUserId();
            var vm = ReportService.BuildReport(_context, userId, Request.StartDate, Request.EndDate, Request.Period);
            var bar = DecodeDataUrl(ChartImageBase64);
            var pie = DecodeDataUrl(PieImageBase64);
            var bytes = ReportExportService.GenerateExcel(vm, bar, pie);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Relatorio.xlsx");
        }

        public IActionResult OnPostExportPdf()
        {
            var userId = GetUserId();
            var vm = ReportService.BuildReport(_context, userId, Request.StartDate, Request.EndDate, Request.Period);
            var bar = DecodeDataUrl(ChartImageBase64);
            var pie = DecodeDataUrl(PieImageBase64);
            var bytes = ReportExportService.GeneratePdf(vm, bar, pie);
            return File(bytes, "application/pdf", "Relatorio.pdf");
        }

        private int GetUserId()
        {
            var s = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(s, out var id) ? id : 1;
        }

        private static byte[]? DecodeDataUrl(string? dataUrl)
        {
            if (string.IsNullOrEmpty(dataUrl)) return null;
            var parts = dataUrl.Split(',');
            if (parts.Length != 2) return null;
            return Convert.FromBase64String(parts[1]);
        }
    }
}