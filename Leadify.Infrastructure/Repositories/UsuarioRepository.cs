using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Leadify.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Usuario>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Usuarios
                .Include(u => u.Perfil) // Incluimos el rol para mostrar el nombre en la tabla
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Nombre.Contains(search) ||
                                         u.Apellido.Contains(search) ||
                                         u.Email.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Usuario>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Perfil)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario, string password)
        {
            // Movimos la lógica de seguridad del controlador al repositorio (Mejor Práctica)
            using var hmac = new HMACSHA512();
            usuario.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            usuario.PasswordSalt = hmac.Key;
            usuario.FechaCreacion = DateTime.Now;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;
            // Evitamos que se pise el password si no estamos editando password acá
            _context.Entry(usuario).Property(x => x.PasswordHash).IsModified = false;
            _context.Entry(usuario).Property(x => x.PasswordSalt).IsModified = false;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false; // Borrado lógico igual que en Clientes
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }
    }
}
