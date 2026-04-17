using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<PagedResult<Cliente>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente> CreateAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(int id);
        Task<bool> ExisteCuilAsync(string cuit);
        Task<bool> ExisteEmailAsync(string mail);

        // Métodos para Sedes (asociadas al cliente)
        Task AddSedeAsync(Sede sede);
        Task<IEnumerable<Sede>> GetSedesByClienteIdAsync(int clienteId);

       

    }
}
