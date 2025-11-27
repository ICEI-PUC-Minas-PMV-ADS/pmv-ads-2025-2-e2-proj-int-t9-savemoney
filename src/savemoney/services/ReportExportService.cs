using System;
using System.IO;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using savemoney.Models;

namespace savemoney.Services
{
    public static class ReportExportService
    {
        public static byte[] GenerateExcel(RelatorioViewModel vm, byte[]? barImg = null, byte[]? pieImg = null)
        {
            using var ms = new MemoryStream();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Relatório");

            ws.Cell(1, 1).Value = "Relatório Financeiro";
            ws.Cell(2, 1).Value = "Período:";
            ws.Cell(2, 2).Value = $"{vm.Request.StartDate:dd/MM/yyyy} - {vm.Request.EndDate:dd/MM/yyyy}";
            ws.Cell(3, 1).Value = "Agregação:";
            ws.Cell(3, 2).Value = vm.Request.Period.ToString();

            ws.Cell(5, 1).Value = "Período";
            ws.Cell(5, 2).Value = "Receitas";
            ws.Cell(5, 3).Value = "Despesas";
            ws.Cell(5, 4).Value = "Saldo";
            int row = 6;
            foreach (var p in vm.Periodos)
            {
                ws.Cell(row, 1).Value = p.Label;
                ws.Cell(row, 2).Value = p.TotalReceitas;
                ws.Cell(row, 3).Value = p.TotalDespesas;
                ws.Cell(row, 4).Value = p.Saldo;
                row++;
            }

            ws.Cell(row + 1, 1).Value = "Total";
            ws.Cell(row + 1, 2).FormulaA1 = $"SUM(B6:B{row - 1})";
            ws.Cell(row + 1, 3).FormulaA1 = $"SUM(C6:C{row - 1})";
            ws.Cell(row + 1, 4).FormulaA1 = $"B{row + 1}-C{row + 1}";

            if (barImg != null)
            {
                using var s = new MemoryStream(barImg);
                ws.AddPicture(s).MoveTo(ws.Cell("F2"));
            }

            if (pieImg != null)
            {
                using var s2 = new MemoryStream(pieImg);
                ws.AddPicture(s2).MoveTo(ws.Cell("F20"));
            }

            wb.SaveAs(ms);
            return ms.ToArray();
        }

        public static byte[] GeneratePdf(RelatorioViewModel vm, byte[]? barImg = null, byte[]? pieImg = null)
        {
            using var ms = new MemoryStream();
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text("Relatório Financeiro").FontSize(16).Bold();
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text($"Período: {vm.Request.StartDate:dd/MM/yyyy} - {vm.Request.EndDate:dd/MM/yyyy}");
                        col.Item().Text($"Agregação: {vm.Request.Period}");

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); cd.RelativeColumn(); cd.RelativeColumn(); });
                            table.Header(h =>
                            {
                                h.Cell().Text("Período");
                                h.Cell().Text("Receitas");
                                h.Cell().Text("Despesas");
                                h.Cell().Text("Saldo");
                            });

                            foreach (var p in vm.Periodos)
                            {
                                table.Cell().Text(p.Label);
                                table.Cell().AlignRight().Text(p.TotalReceitas.ToString("N2"));
                                table.Cell().AlignRight().Text(p.TotalDespesas.ToString("N2"));
                                table.Cell().AlignRight().Text(p.Saldo.ToString("N2"));
                            }
                        });

                        if (barImg != null) col.Item().Image(barImg).FitWidth();
                        if (pieImg != null) col.Item().Image(pieImg).FitWidth();

                        col.Item().PaddingTop(8).Text($"Total Receitas: {vm.TotalReceitas:N2}    Total Despesas: {vm.TotalDespesas:N2}    Saldo: {vm.Saldo:N2}");
                    });

                    page.Footer().AlignCenter().Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}