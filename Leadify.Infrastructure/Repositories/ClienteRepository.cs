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
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Ahora buscamos por Nombre, Apellido o CUIL
                query = query.Where(c =>
                    c.Nombre.Contains(search) ||
                    c.Apellido.Contains(search) ||
                    c.CUIL.Contains(search) ||
                    c.DNI.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                // Ordenamos por Apellido y luego por Nombre
                .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
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
                // Mantenemos el borrado lógico para integridad referencial
                cliente.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        // Actualizado a CUIL
        public async Task<bool> ExisteCuilAsync(string cuil)
        {
            if (string.IsNullOrEmpty(cuil)) return false;
            return await _context.Clientes.AnyAsync(c => c.CUIL == cuil);
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