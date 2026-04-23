using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface IArticuloInternoRepository
    {
        Task<PagedResult<ArticulosInternos>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<ArticulosInternos?> GetByIdAsync(int id);
        Task<ArticulosInternos> CreateAsync(ArticulosInternos articulo);
        Task UpdateAsync(ArticulosInternos articulo);
        Task DeleteAsync(int id);
        Task<bool> ExisteCodigoAsync(string codigo);
    }
}
