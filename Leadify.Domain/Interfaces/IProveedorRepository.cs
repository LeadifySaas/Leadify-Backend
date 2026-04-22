using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface IProveedorRepository
    {
        Task<PagedResult<Proveedor>> GetPagedAsync(int pageIndex, int pageSize, string? search);
        Task<Proveedor?> GetByIdAsync(int id);
        Task<Proveedor> CreateAsync(Proveedor cliente);
        Task UpdateAsync(Proveedor cliente);
        Task DeleteAsync(int id);

        Task UpdateArchivo(ProveedorArchivo cliente);
        Task<bool> ExisteCuitAsync(string cuit);
        Task<bool> ExisteEmailAsync(string mail);
        Task SaveArchivoAsync(ProveedorArchivo archivo);
        Task<List<ProveedorArchivo>> GetArchivosByProveedorId(int proveedorId);
        Task<ProveedorArchivo?> GetArchivoById(int archivoId);
        Task DeleteArchivo(ProveedorArchivo archivo);
    }


}
