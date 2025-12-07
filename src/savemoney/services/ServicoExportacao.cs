using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using savemoney.Models;
using System.Globalization;
using System.Linq; // Necessário para o Sum()

namespace savemoney.Services
{
    public class ServicoExportacao
    {
        private readonly CultureInfo _cultura = new CultureInfo("pt-BR");

        // ==========================================
        // 1. EXCEL (ClosedXML)
        // ==========================================

        public byte[] GerarExcel(List<TransacaoProcessada> dados, DateTime inicio, DateTime fim)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Extrato");

            // Cores
            var corFundo = XLColor.FromHtml("#10111a");
            var corTexto = XLColor.White;

            // Cabeçalho
            ws.Cell(1, 1).Value = "SAVEMONEY - EXTRATO FINANCEIRO";
            var tituloRange = ws.Range(1, 1, 1, 6).Merge();
            tituloRange.Style.Font.FontSize = 14;
            tituloRange.Style.Font.Bold = true;
            tituloRange.Style.Font.FontColor = corTexto;
            tituloRange.Style.Fill.BackgroundColor = corFundo;
            tituloRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(2, 1).Value = $"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}";
            var subTituloRange = ws.Range(2, 1, 2, 6).Merge();
            subTituloRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Colunas
            var headers = new[] { "Data", "Dia", "Tipo", "Descrição", "Categoria", "Valor" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(4, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.Gray;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Dados
            int row = 5;
            foreach (var item in dados.OrderByDescending(x => x.Data))
            {
                ws.Cell(row, 1).Value = item.Data;
                ws.Cell(row, 2).Value = item.DiaSemana;
                ws.Cell(row, 3).Value = item.EhReceita ? "Receita" : "Despesa";
                ws.Cell(row, 4).Value = item.Descricao;
                ws.Cell(row, 5).Value = item.Categoria;
                ws.Cell(row, 6).Value = item.Valor;

                var corValor = item.EhReceita ? XLColor.SeaGreen : XLColor.IndianRed;
                ws.Cell(row, 3).Style.Font.FontColor = corValor;
                ws.Cell(row, 6).Style.Font.FontColor = corValor;
                ws.Cell(row, 6).Style.NumberFormat.Format = "R$ #,##0.00";

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GerarExcelCompleto(
            List<Receita> receitas,
            List<Despesa> despesas,
            List<Budget> orcamentos,
            List<MetaFinanceira> metas)
        {
            using var workbook = new XLWorkbook();

            // --- ABA 1: RESUMO ---
            var wsResumo = workbook.Worksheets.Add("Visão Geral");
            wsResumo.Cell(1, 1).Value = "RESUMO DA CONTA";
            var resRange = wsResumo.Range(1, 1, 1, 2).Merge();
            resRange.Style.Font.Bold = true;
            resRange.Style.Font.FontSize = 14;

            wsResumo.Cell(3, 1).Value = "Total Receitas Cadastradas:";
            wsResumo.Cell(3, 2).Value = receitas.Count;

            wsResumo.Cell(4, 1).Value = "Total Despesas Cadastradas:";
            wsResumo.Cell(4, 2).Value = despesas.Count;

            wsResumo.Cell(6, 1).Value = "Valor Total Entradas:";
            wsResumo.Cell(6, 2).Value = receitas.Sum(x => x.Valor);
            wsResumo.Cell(6, 2).Style.NumberFormat.Format = "R$ #,##0.00";

            wsResumo.Cell(7, 1).Value = "Valor Total Saídas:";
            wsResumo.Cell(7, 2).Value = despesas.Sum(x => x.Valor);
            wsResumo.Cell(7, 2).Style.NumberFormat.Format = "R$ #,##0.00";

            wsResumo.Columns().AdjustToContents();

            // --- ABA 2: RECEITAS ---
            if (receitas.Any())
            {
                var wsRec = workbook.Worksheets.Add("Receitas");
                CriarCabecalhoExcel(wsRec, "Data", "Descrição", "Valor", "Recorrente?");
                int row = 2;
                foreach (var item in receitas.OrderByDescending(x => x.DataInicio))
                {
                    wsRec.Cell(row, 1).Value = item.DataInicio;
                    wsRec.Cell(row, 2).Value = item.Titulo;
                    wsRec.Cell(row, 3).Value = item.Valor;
                    wsRec.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    wsRec.Cell(row, 4).Value = item.IsRecurring ? "Sim" : "Não";
                    row++;
                }
                wsRec.Columns().AdjustToContents();
            }

            // --- ABA 3: DESPESAS ---
            if (despesas.Any())
            {
                var wsDesp = workbook.Worksheets.Add("Despesas");
                CriarCabecalhoExcel(wsDesp, "Data", "Descrição", "Valor", "Categoria", "Status");
                int row = 2;
                foreach (var item in despesas.OrderByDescending(x => x.DataInicio))
                {
                    wsDesp.Cell(row, 1).Value = item.DataInicio;
                    wsDesp.Cell(row, 2).Value = item.Titulo;
                    wsDesp.Cell(row, 3).Value = item.Valor;
                    wsDesp.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    wsDesp.Cell(row, 4).Value = item.BudgetCategory?.Category?.Name ?? "Sem Categoria";
                    wsDesp.Cell(row, 5).Value = item.Pago ? "Pago" : "Pendente";

                    if (!item.Pago) wsDesp.Cell(row, 5).Style.Font.FontColor = XLColor.Red;

                    row++;
                }
                wsDesp.Columns().AdjustToContents();
            }

            // --- ABA 4: ORÇAMENTOS ---
            if (orcamentos.Any())
            {
                var wsBud = workbook.Worksheets.Add("Orçamentos");
                CriarCabecalhoExcel(wsBud, "Nome", "Início", "Fim", "Limite Estimado"); // Alterado título
                int row = 2;
                foreach (var item in orcamentos)
                {
                    wsBud.Cell(row, 1).Value = item.Name;
                    wsBud.Cell(row, 2).Value = item.StartDate;
                    wsBud.Cell(row, 3).Value = item.EndDate;

                    // CORREÇÃO SEGURA: Soma categorias ou 0 se nulo. Removemos item.TotalLimit
                    var limiteTotal = item.Categories?.Sum(c => c.Limit) ?? 0;
                    wsBud.Cell(row, 4).Value = limiteTotal;

                    wsBud.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    row++;
                }
                wsBud.Columns().AdjustToContents();
            }

            // --- ABA 5: METAS ---
            if (metas.Any())
            {
                var wsMetas = workbook.Worksheets.Add("Metas Financeiras");
                // CORREÇÃO SEGURA: Removemos coluna "Prazo"
                CriarCabecalhoExcel(wsMetas, "Objetivo", "Valor Alvo");
                int row = 2;
                foreach (var item in metas)
                {
                    wsMetas.Cell(row, 1).Value = item.Titulo;
                    wsMetas.Cell(row, 2).Value = item.ValorObjetivo;
                    wsMetas.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                    // CORREÇÃO: Removemos item.DataFim
                    row++;
                }
                wsMetas.Columns().AdjustToContents();
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void CriarCabecalhoExcel(IXLWorksheet ws, params string[] titulos)
        {
            for (int i = 0; i < titulos.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = titulos[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#10111a");
                cell.Style.Font.FontColor = XLColor.White;
            }
        }

        // ==========================================
        // 2. PDF (QuestPDF)
        // ==========================================

        public byte[] GerarPdf(List<TransacaoProcessada> dados, DateTime inicio, DateTime fim, Usuario usuario)
        {
            var totalEntrada = dados.Where(x => x.EhReceita).Sum(x => x.Valor);
            var totalSaida = dados.Where(x => !x.EhReceita).Sum(x => x.Valor);

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Cabeçalho
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SaveMoney").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Relatório Gerencial").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Gerado por: {usuario.Nome}");
                            col.Item().Text($"{DateTime.Now:g}");
                        });
                    });

                    // Conteúdo
                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        // Resumo
                        col.Item().Row(r =>
                        {
                            r.RelativeItem().Component(new BoxResumo("Entradas", totalEntrada, Colors.Green.Medium, _cultura));
                            r.ConstantItem(10);
                            r.RelativeItem().Component(new BoxResumo("Saídas", totalSaida, Colors.Red.Medium, _cultura));
                            r.ConstantItem(10);
                            r.RelativeItem().Component(new BoxResumo("Resultado", totalEntrada - totalSaida, Colors.Blue.Medium, _cultura));
                        });

                        col.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        // Tabela
                        col.Item().Table(table =>
                        {
                            // CORREÇÃO DO QUESTPDF: RelativeItem() -> RelativeColumn()
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(60);
                                c.RelativeColumn(); // <--- AQUI ESTAVA O ERRO DE SINTAXE
                                c.ConstantColumn(100);
                                c.ConstantColumn(80);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(CellStyleHeader).Text("Data");
                                h.Cell().Element(CellStyleHeader).Text("Descrição");
                                h.Cell().Element(CellStyleHeader).Text("Categoria");
                                h.Cell().Element(CellStyleHeader).AlignRight().Text("Valor");
                            });

                            foreach (var item in dados.OrderByDescending(x => x.Data))
                            {
                                table.Cell().Element(CellStyleRow).Text($"{item.Data:dd/MM}");
                                table.Cell().Element(CellStyleRow).Text(item.Descricao);
                                table.Cell().Element(CellStyleRow).Text(item.Categoria);

                                var cor = item.EhReceita ? Colors.Green.Darken2 : Colors.Red.Darken2;
                                var sinal = item.EhReceita ? "+" : "-";
                                table.Cell().Element(CellStyleRow).AlignRight().Text($"{sinal} {item.Valor.ToString("N2", _cultura)}").FontColor(cor);
                            }
                        });
                    });

                    // Rodapé
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return documento.GeneratePdf();
        }

        public byte[] GerarPdfCompleto(
            List<Receita> receitas,
            List<Despesa> despesas,
            List<Budget> orcamentos,
            List<MetaFinanceira> metas,
            Usuario usuario)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Cabeçalho
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SaveMoney").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Dossiê Financeiro Completo").FontSize(12).FontColor(Colors.Grey.Medium);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Titular: {usuario.Nome}");
                            col.Item().Text($"{DateTime.Now:dd/MM/yyyy}");
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Seção 1: Resumo
                        col.Item().Text("Resumo Geral").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().PaddingTop(5).Row(r =>
                        {
                            r.RelativeItem().Text($"Total Receitas: {receitas.Count}");
                            r.RelativeItem().Text($"Total Despesas: {despesas.Count}");
                            r.RelativeItem().Text($"Metas Ativas: {metas.Count}");
                        });

                        col.Item().PaddingBottom(20);

                        // Seção 2: Últimas Movimentações
                        col.Item().Text("Últimas 20 Movimentações").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        var movimentacoes = receitas.Select(x => new { Data = x.DataInicio, Desc = x.Titulo, Valor = x.Valor, Tipo = "Receita" })
                            .Concat(despesas.Select(x => new { Data = x.DataInicio, Desc = x.Titulo, Valor = x.Valor, Tipo = "Despesa" }))
                            .OrderByDescending(x => x.Data)
                            .Take(20);

                        col.Item().PaddingTop(10).Table(table =>
                        {
                            // CORREÇÃO DO QUESTPDF: RelativeItem() -> RelativeColumn()
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(70);
                                c.ConstantColumn(70);
                                c.RelativeColumn(); // <--- AQUI ESTAVA O ERRO DE SINTAXE
                                c.ConstantColumn(90);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(CellStyleHeader).Text("Data");
                                h.Cell().Element(CellStyleHeader).Text("Tipo");
                                h.Cell().Element(CellStyleHeader).Text("Descrição");
                                h.Cell().Element(CellStyleHeader).AlignRight().Text("Valor");
                            });

                            foreach (var mov in movimentacoes)
                            {
                                table.Cell().Element(CellStyleRow).Text($"{mov.Data:dd/MM/yy}");
                                table.Cell().Element(CellStyleRow).Text(mov.Tipo).FontColor(mov.Tipo == "Receita" ? Colors.Green.Darken1 : Colors.Red.Darken1);
                                table.Cell().Element(CellStyleRow).Text(mov.Desc);
                                table.Cell().Element(CellStyleRow).AlignRight().Text(mov.Valor.ToString("C", _cultura));
                            }
                        });

                        // Seção 3: Metas
                        if (metas.Any())
                        {
                            col.Item().PaddingTop(20).Text("Metas Financeiras").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            foreach (var meta in metas)
                            {
                                col.Item().PaddingTop(5).Row(r =>
                                {
                                    r.RelativeItem().Text(meta.Titulo).SemiBold();
                                    r.RelativeItem().AlignRight().Text($"Alvo: {meta.ValorObjetivo.ToString("C", _cultura)}");
                                });
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x => x.CurrentPageNumber());
                });
            });

            return documento.GeneratePdf();
        }

        static IContainer CellStyleHeader(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Background(Colors.Grey.Lighten4);

        static IContainer CellStyleRow(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5);
    }

    public class BoxResumo : IComponent
    {
        private string Titulo;
        private decimal Valor;
        private string Cor;
        private CultureInfo Cultura;

        public BoxResumo(string t, decimal v, string c, CultureInfo ci) { Titulo = t; Valor = v; Cor = c; Cultura = ci; }

        public void Compose(IContainer container)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
            {
                c.Item().Text(Titulo).FontSize(9).FontColor(Colors.Grey.Darken1);
                c.Item().Text(Valor.ToString("C", Cultura)).FontSize(14).SemiBold().FontColor(Cor);
            });
        }
    }
}