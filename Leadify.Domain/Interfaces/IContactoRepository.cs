using Leadify.Domain.Common;
using Leadify.Domain.Entities;

namespace Leadify.Domain.Interfaces
{
    public interface IContactoRepository
    {
        Task<PagedResult<Contacto>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<IEnumerable<Contacto>> GetByEmpresaIdAsync(int empresaId);
        Task<IEnumerable<Contacto>> GetByClienteIdAsync(int clienteId);
        Task<Contacto?> GetByIdAsync(int id);
        Task<Contacto> CreateAsync(Contacto contacto);
        Task UpdateAsync(Contacto contacto);
        Task DeleteAsync(int id);

        Task<bool> ExisteEmailAsync(string mail);
    }
}