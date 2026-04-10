using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data; 
using Microsoft.EntityFrameworkCore;

namespace Leadify.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Cliente>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Clientes
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.RazonSocial.Contains(search) || c.CUIT.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.RazonSocial).ThenByDescending(a => a.Activo)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Cliente>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cliente> CreateAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false; // Borrado lógico para no romper historial de remitos
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteCuitAsync(string cuit)
        {
            return await _context.Clientes.AnyAsync(c => c.CUIT == cuit);
        }

        // --- Lógica de Sedes ---
        public async Task AddSedeAsync(Sede sede)
        {
            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Sede>> GetSedesByClienteIdAsync(int clienteId)
        {
            return await _context.Sedes
                .Where(s => s.ClienteId == clienteId)
                .ToListAsync();
        }
    }
}