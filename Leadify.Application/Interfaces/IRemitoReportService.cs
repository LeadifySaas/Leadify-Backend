// Leadify.Application/Interfaces/IRemitoReportService.cs
using Leadify.Domain.Entities;

namespace Leadify.Application.Interfaces;

public interface IRemitoReportService
{
    byte[] GenerarPdf(Remito remito);
    byte[] GenerarExcel(IEnumerable<Remito> remitos);
}