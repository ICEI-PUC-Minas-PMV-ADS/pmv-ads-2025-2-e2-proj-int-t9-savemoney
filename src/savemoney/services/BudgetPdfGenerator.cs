// Services/BudgetPdfGenerator.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using savemoney.Models;

namespace savemoney.Services
{
    public class BudgetPdfGenerator
    {
        public byte[] GeneratePdf(Budget budget)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // HEADER — SÓ UM!
                    page.Header()
                        .Column(column =>
                        {
                            column.Item().AlignCenter().Text("SAVE MONEY").FontSize(28).SemiBold().FontColor(Colors.Blue.Darken3);
                            column.Item().AlignCenter().Text(budget.Name).FontSize(20).SemiBold().FontColor(Colors.Grey.Darken3);
                            column.Item().AlignCenter().Text($"{budget.StartDate:dd/MM/yyyy} até {budget.EndDate:dd/MM/yyyy}")
                                .FontSize(14).FontColor(Colors.Grey.Darken2);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item().Text("Resumo por Categoria").FontSize(16).SemiBold();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                // HEADER
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Categoria").SemiBold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Limite").SemiBold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Gasto").SemiBold().AlignRight();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("%").SemiBold().AlignRight();
                                });

                                var totalLimit = budget.Categories.Sum(c => c.Limit);
                                var totalSpent = budget.Categories.Sum(c => c.CurrentSpent);

                                foreach (var bc in budget.Categories)
                                {
                                    var percent = bc.Limit > 0 ? (bc.CurrentSpent / bc.Limit) * 100 : 0;
                                    var color = percent > 100 ? Colors.Red.Medium : percent > 80 ? Colors.Orange.Medium : Colors.Green.Medium;

                                    table.Cell().Padding(5).Text(bc.Category.Name);
                                    table.Cell().Padding(5).Text($"R$ {bc.Limit:F2}").AlignRight();
                                    table.Cell().Padding(5).Text($"R$ {bc.CurrentSpent:F2}").AlignRight().FontColor(color);
                                    table.Cell().Padding(5).Text($"{percent:F1}%").AlignRight().FontColor(color);
                                }

                                // TOTAL
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("TOTAL").SemiBold();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text($"R$ {totalLimit:F2}").SemiBold().AlignRight();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text($"R$ {totalSpent:F2}").SemiBold().AlignRight();
                                table.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text($"{(totalLimit > 0 ? (totalSpent / totalLimit) * 100 : 0):F1}%").SemiBold().AlignRight();
                            });

                            column.Item().AlignCenter().PaddingTop(30)
                                .Text($"Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(10).FontColor(Colors.Grey.Medium);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                });
            }).GeneratePdf();
        }
    }
}