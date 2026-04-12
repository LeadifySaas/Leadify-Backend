using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Leadify.Infrastructure.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly ApplicationDbContext _context;

        public EmpresaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Empresa>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Empresas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Buscamos por Razón Social o CUIT
                query = query.Where(e =>
                    e.RazonSocial.Contains(search) ||
                    e.Cuit.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.RazonSocial)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Empresa>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Empresa>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Empresas
                .Where(e => e.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<Empresa?> GetByIdAsync(int id)
        {
            return await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Empresa> CreateAsync(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task UpdateAsync(Empresa empresa)
        {
            _context.Entry(empresa).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var empresa = await _context.Empresas.FindAsync(id);
            if (empresa != null)
            {
                // Borrado lógico para no romper la relación con Clientes
                empresa.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteCuitAsync(string cuit)
        {
            if (string.IsNullOrEmpty(cuit)) return false;
            return await _context.Empresas.AnyAsync(e => e.Cuit == cuit);
        }
    }
}