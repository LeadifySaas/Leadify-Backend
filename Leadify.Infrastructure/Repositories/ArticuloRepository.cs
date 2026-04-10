using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Infrastructure.Repositories
{
    public class ArticuloRepository : IArticuloRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticuloRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Articulo>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Articulos
                .AsQueryable();

            // Filtro de búsqueda (Buscamos por Razón Social o CUIT)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Nombre.Contains(search) || c.Codigo.Contains(search));
            }

            // 1. Contamos el total bajo ese filtro
            var totalCount = await query.CountAsync();

            // 2. Aplicamos paginación
            var items = await query
                .OrderBy(c => c.Nombre)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 3. Devolvemos el envoltorio
            return new PagedResult<Articulo>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<Articulo?> GetByIdAsync(int id)
        {
            return await _context.Articulos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Articulo> CreateAsync(Articulo articulo)
        {
            _context.Articulos.Add(articulo);
            await _context.SaveChangesAsync();
            return articulo;
        }

        public async Task UpdateAsync(Articulo articulo)
        {
            _context.Entry(articulo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo != null)
            {
                articulo.Activo = false; 
                articulo.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteCodigoAsync(string codigo)
        {
            return await _context.Articulos.AnyAsync(c => c.Codigo == codigo);
        }

        
    }
}
