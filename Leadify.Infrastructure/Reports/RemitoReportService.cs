using ClosedXML.Excel;
using Leadify.Application.Interfaces;
using Leadify.Domain.Entities;
using Leadify.Infrastructure.Data.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace Leadify.Infrastructure.Reports;

public class RemitoReportService : IRemitoReportService
{
    public byte[] GenerarPdf(Remito remito)
    {
        // Aquí instanciamos la plantilla que creamos arriba
        var report = new RemitoPdfTemplate(remito);
        return report.GeneratePdf();
    }
    public byte[] GenerarExcel(IEnumerable<Remito> remitos)
    {
        // Aquí inicializamos el workbook de ClosedXML
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Remitos");

        // Cabeceras
        worksheet.Cell(1, 1).Value = "Número";
        worksheet.Cell(1, 2).Value = "Fecha";
        worksheet.Cell(1, 3).Value = "Cliente";
        worksheet.Cell(1, 4).Value = "Estado";

        int row = 2;
        foreach (var r in remitos)
        {
            worksheet.Cell(row, 1).Value = r.NumeroRemito;
            worksheet.Cell(row, 2).Value = r.FechaEmision;
            worksheet.Cell(row, 3).Value = r.Cliente?.RazonSocial;
            worksheet.Cell(row, 4).Value = r.Estado;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        // Creamos el "stream" que te faltaba
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}