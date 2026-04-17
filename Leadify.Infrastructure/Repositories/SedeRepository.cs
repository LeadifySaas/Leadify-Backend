using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore;

namespace Leadify.Infrastructure.Repositories
{
    public class SedeRepository : ISedeRepository
    {
        private readonly ApplicationDbContext _context;

        public SedeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Sede>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Sedes
                .Include(s => s.Cliente) 
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Nombre.Contains(search) ||
                                         s.Localidad.Contains(search) ||
                                         s.Cliente.RazonSocial.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(s => s.Nombre)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Sede> { Items = items, TotalCount = totalCount, PageIndex = pageIndex, PageSize = pageSize };
        }

        public async Task<Sede?> GetByIdAsync(int id)
        {
            return await _context.Sedes
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sede> CreateAsync(Sede sede)
        {
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();
            return sede;
        }

        public async Task UpdateAsync(Sede sede)
        {
            _context.Entry(sede).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sede = await _context.Sedes.FindAsync(id);
            if (sede != null)
            {
                sede.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteNombreAsync(string nombre)
        {
            return await _context.Sedes.AnyAsync(s => s.Nombre == nombre && s.Activo);
        }
    }
}