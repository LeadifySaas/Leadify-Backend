using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Leadify.Infrastructure.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProveedorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Proveedor>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Proveedores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Buscamos por RazonSocial o CUIT
                query = query.Where(p =>
                    p.RazonSocial.Contains(search) ||
                    p.Cuit.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                // Ordenamos por Razón Social
                .OrderBy(p => p.RazonSocial)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Proveedor>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Proveedor?> GetByIdAsync(int id)
        {
            return await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Proveedor> CreateAsync(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return proveedor;
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                // Borrado lógico
                proveedor.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteCuitAsync(string cuit)
        {
            if (string.IsNullOrEmpty(cuit)) return false;
            return await _context.Proveedores.AnyAsync(p => p.Cuit == cuit);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return await _context.Proveedores.AnyAsync(p => p.Email.ToLower() == email.ToLower());
        }

        public async Task<List<ProveedorArchivo>> GetArchivosByProveedorId(int proveedorId)
        {
            return await _context.ProveedorArchivos
            .Where(a => a.ProveedorId == proveedorId && a.Activo == true) 
            .ToListAsync();
        }

        public async Task<ProveedorArchivo?> GetArchivoById(int archivoId)
        {
            return await _context.ProveedorArchivos
                .FirstOrDefaultAsync(a => a.Id == archivoId);
        }

        public async Task SaveArchivoAsync(ProveedorArchivo archivo)
        {
            _context.ProveedorArchivos.Add(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteArchivo(ProveedorArchivo archivo)
        {
            _context.ProveedorArchivos.Remove(archivo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArchivo(ProveedorArchivo archivo) { 
            _context.ProveedorArchivos.Update(archivo);
            await _context.SaveChangesAsync();
        }
    }
}