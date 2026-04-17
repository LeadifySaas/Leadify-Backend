using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Interfaces
{
    public interface IRemitoRepository
    {
        Task<PagedResult<Remito>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Remito?> GetByIdAsync(int id);
        Task<Remito> CreateAsync(Remito remito);
        Task UpdateAsync(Remito remito);
        Task UpdateEstadoAsync(int id, string nuevoEstado);
        Task AnularAsync(int id);
    }
}
