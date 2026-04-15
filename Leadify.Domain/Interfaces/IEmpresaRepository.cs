using Leadify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Leadify.Domain.Entities;

namespace Leadify.Domain.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<PagedResult<Empresa>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<IEnumerable<Empresa>> GetByClienteIdAsync(int clienteId);
        Task<Empresa?> GetByIdAsync(int id);
        Task<Empresa> CreateAsync(Empresa empresa);
        Task UpdateAsync(Empresa empresa);
        Task DeleteAsync(int id);
        Task<bool> ExisteCuitAsync(string cuit);

        Task<bool> ExisteEmailAsync(string mail);
    }

}
