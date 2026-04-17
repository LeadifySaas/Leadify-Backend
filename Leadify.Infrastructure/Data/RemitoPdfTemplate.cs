using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Leadify.Domain.Entities;

namespace Leadify.Infrastructure.Data.Reports
{
    public class RemitoPdfTemplate : IDocument
    {
        private readonly Remito _remito;

        public RemitoPdfTemplate(Remito remito)
        {
            _remito = remito;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);

                // --- MARCA DE AGUA ---
                if (_remito.Estado?.ToLower() == "anulado")
                {
                    page.Foreground().AlignCenter().AlignMiddle().Rotate(-45).Text("ANULADO")
                        .FontSize(100).SemiBold().FontColor(Colors.Red.Lighten3);
                }

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter); // Footer separado para control total
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row => {
                row.RelativeItem().Column(col => {
                    col.Item().Text("BISIACH").FontSize(24).ExtraBold().FontColor("#1A365D");
                    col.Item().Text("Aluminios & Vidrios").FontSize(12).SemiBold().Italic().FontColor(Colors.Grey.Medium);

                    col.Item().PaddingTop(5).Column(c => {
                        c.Item().Text("H. IRIGOYEN ESQ. PEDERNERA (5777)").FontSize(8);
                        c.Item().Text("SANTA ROSA DEL CONLARA - SAN LUÍS").FontSize(8);
                        c.Item().Text("Tel: 2656492391 | bisiachvidrios@hotmail.com").FontSize(8);
                    });
                });

                row.RelativeItem().AlignRight().Column(col => {
                    col.Item().Text("REMITO").FontSize(22).ExtraBold().FontColor("#1A365D");

                    col.Item().PaddingTop(2).Background(Colors.Grey.Lighten4).Padding(5).Column(c => {
                        c.Item().Text($"Número: {_remito.NumeroRemito}").FontSize(11).SemiBold();
                        c.Item().Text($"Fecha: {_remito.FechaEmision:dd/MM/yyyy}").FontSize(11);
                    });
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            decimal totalGeneral = 0;

            container.PaddingVertical(20).Column(col => {

                // Bloque Cliente
                col.Item().Row(row => {
                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c => {
                        c.Item().Text("DESTINATARIO").FontSize(9).SemiBold().FontColor(Colors.Grey.Medium);
                        c.Item().Text(_remito.Cliente?.RazonSocial ?? "S/D").FontSize(12).SemiBold();
                        c.Item().Text($"Obra: {_remito.Sede?.Nombre ?? "S/D"}").FontSize(10);
                    });
                });

                col.Item().PaddingTop(15);

                // Tabla con estilo heredado
                col.Item().DefaultTextStyle(x => x.FontSize(10)).Table(table => {
                    table.ColumnsDefinition(columns => {
                        columns.ConstantColumn(45);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    table.Header(header => {
                        header.Cell().Element(HeaderStyle).AlignCenter().Text("CANT.");
                        header.Cell().Element(HeaderStyle).Text("DETALLE / TIPOLOGÍAS");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("UNITARIO");
                        header.Cell().Element(HeaderStyle).AlignRight().Text("TOTAL");

                        static IContainer HeaderStyle(IContainer c) =>
                            c.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).PaddingHorizontal(5)
                             .Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten1);
                    });

                    foreach (var item in _remito.Items)
                    {
                        var precio = item.Articulo?.PrecioVenta ?? 0;
                        var totalItem = item.Cantidad * precio;
                        totalGeneral += totalItem;

                        table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());
                        table.Cell().Element(CellStyle).Column(c => {
                            c.Item().Text(item.Articulo?.Nombre ?? "Sin Nombre").SemiBold();
                            if (!string.IsNullOrEmpty(item.Notas))
                                c.Item().Text(item.Notas).FontSize(9).Italic().FontColor(Colors.Grey.Darken2);
                        });
                        table.Cell().Element(CellStyle).AlignRight().Text(precio.ToString("C"));
                        table.Cell().Element(CellStyle).AlignRight().Text(totalItem.ToString("C"));

                        static IContainer CellStyle(IContainer c) =>
                            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten4).PaddingVertical(8).PaddingHorizontal(5);
                    }
                });

                // Totales y Observaciones
                col.Item().PaddingTop(10).Row(row => {
                    row.RelativeItem().Column(c => {
                        if (!string.IsNullOrEmpty(_remito.Observaciones))
                        {
                            c.Item().Text("OBSERVACIONES:").FontSize(9).SemiBold();
                            c.Item().Text(_remito.Observaciones).FontSize(9);
                        }
                    });

                    row.ConstantItem(150).Column(c => {
                        c.Item().BorderTop(1).PaddingTop(5).Row(r => {
                            r.RelativeItem().Text("TOTAL:").SemiBold();
                            r.RelativeItem().AlignRight().Text(totalGeneral.ToString("C")).SemiBold();
                        });
                    });
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col => {
                // DOBLE FIRMA: Una a la izquierda y otra a la derecha
                col.Item().PaddingBottom(20).Row(row => {
                    // Firma Emisor
                    row.RelativeItem().Column(c => {
                        c.Item().PaddingTop(40).BorderTop(1).AlignCenter().Text("Firma Autorizada").FontSize(9);
                        c.Item().AlignCenter().Text("BISIACH Aluminios").FontSize(7).FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(50); // Espacio entre firmas

                    // Firma Receptor
                    row.RelativeItem().Column(c => {
                        c.Item().PaddingTop(40).BorderTop(1).AlignCenter().Text("Recibí Conforme").FontSize(9);
                        c.Item().AlignCenter().Text("Aclaración y DNI").FontSize(7).FontColor(Colors.Grey.Medium);
                    });
                });

                col.Item().AlignCenter().Text(x => {
                    x.Span("Página ").FontSize(9);
                    x.CurrentPageNumber().FontSize(9);
                    x.Span(" de ").FontSize(9);
                    x.TotalPages().FontSize(9);
                });
            });
        }
    }
}