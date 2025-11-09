using System;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SaveMonney.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromName { get; set; } = "SaveMonney";
        public string FromEmail { get; set; } = "no-reply@example.com";
    }

    public class ReportService
    {
        private readonly SmtpOptions _smtp;

        public ReportService(SmtpOptions smtpOptions)
        {
            _smtp = smtpOptions;
        }

        public Task<byte[]> GenerateExcelAsync()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Relatório");
            // Cabeçalho 
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Descrição";
            ws.Cell(1, 3).Value = "Valor";
            // Linha exemplo
            ws.Cell(2, 1).Value = 1;
            ws.Cell(2, 2).Value = "Exemplo";
            ws.Cell(2, 3).Value = 123.45;
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return Task.FromResult(ms.ToArray());
        }

        public Task<byte[]> GeneratePdfAsync()
        {
        // Exemplo simples com QuestPDF
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Header().Text("Relatório SaveMonney").SemiBold().FontSize(20);
                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    col.Item().Text("Linha 1: exemplo de relatório.");
                    col.Item().Text("Linha 2: valores e resumo.");
                });
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Gerado em ");
                    text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).Style(TextStyle.Default.Size(10));
                });
            });

        });

        using var ms = new MemoryStream();
        doc.GeneratePdf(ms);
        return Task.FromResult(ms.ToArray());
    }
}


}
