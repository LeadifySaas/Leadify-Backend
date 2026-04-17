using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Leadify.Infrastructure.Repositories
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Contacto>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Contactos
                .Include(c => c.Empresa) // Incluimos para saber de qué empresa es en la grilla
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(search) ||
                    c.Apellido.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.Puesto.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Contacto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Contacto>> GetByEmpresaIdAsync(int empresaId)
        {
            return await _context.Contactos
                .Where(c => c.EmpresaId == empresaId)
                .OrderBy(c => c.Apellido)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contacto>> GetByClienteIdAsync(int clienteId)
        {
            // Trae contactos asociados directamente al cliente (que no tienen empresa)
            return await _context.Contactos
                .Where(c => c.ClienteId == clienteId && c.EmpresaId == null)
                .OrderBy(c => c.Apellido)
                .ToListAsync();
        }

        public async Task<Contacto?> GetByIdAsync(int id)
        {
            return await _context.Contactos
                .Include(c => c.Empresa)
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contacto> CreateAsync(Contacto contacto)
        {
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
            return contacto;
        }

        public async Task UpdateAsync(Contacto contacto)
        {
            _context.Entry(contacto).State = EntityState.Modified;
            // Aquí es donde en el futuro actualizaríamos FechaModificacion manualmente 
            // si no lo hacemos automático en el SaveChanges
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto != null)
            {
                // Borrado lógico consistente con el resto del sistema
                contacto.Activo = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            // Usamos ToLower() para que la validación sea case-insensitive
            return await _context.Contactos.AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }
    }
}