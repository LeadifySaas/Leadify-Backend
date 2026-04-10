using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface ISedeRepository
    {
        Task<PagedResult<Sede>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Sede?> GetByIdAsync(int id);
        Task<Sede> CreateAsync(Sede Sede);
        Task UpdateAsync(Sede Sede);
        Task DeleteAsync(int id);
        Task<bool> ExisteNombreAsync(string codigo);
    }
}
