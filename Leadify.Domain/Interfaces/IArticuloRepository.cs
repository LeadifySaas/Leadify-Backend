using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface IArticuloRepository
    {
        Task<PagedResult<Articulo>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Articulo?> GetByIdAsync(int id);
        Task<Articulo> CreateAsync(Articulo articulo);
        Task UpdateAsync(Articulo articulo);
        Task DeleteAsync(int id);
        Task<bool> ExisteCodigoAsync(string codigo);
    }
}
