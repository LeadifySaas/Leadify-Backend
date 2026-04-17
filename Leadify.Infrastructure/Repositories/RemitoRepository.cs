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
    public class RemitoRepository : IRemitoRepository
    {
        private readonly ApplicationDbContext _context;

        public RemitoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Remito>> GetPagedAsync(int pageIndex, int pageSize, string? search)
        {
            var query = _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.Sede)
                .Include(r => r.CreadoPorUsuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Buscamos por número de remito o nombre del cliente
                query = query.Where(r => r.NumeroRemito.Contains(search) ||
                                         r.Cliente.RazonSocial.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(r => r.FechaEmision) // Los últimos remitos primero
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Remito> { Items = items, TotalCount = totalCount, PageIndex = pageIndex, PageSize = pageSize };
        }

        public async Task<Remito?> GetByIdAsync(int id)
        {
            return await _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.Sede)
                .Include(r => r.Items)
                    .ThenInclude(i => i.Articulo) 
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Remito> CreateAsync(Remito remito)
        {
            // Usamos una transacción para asegurar integridad total
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar duplicado de Número de Remito (Evita errores de DB feos)
                bool existe = await _context.Remitos.AnyAsync(r => r.NumeroRemito == remito.NumeroRemito);
                if (existe) throw new Exception($"El número de remito {remito.NumeroRemito} ya existe.");

                foreach (var item in remito.Items)
                {
                    // 2. Traer el artículo para validar stock (con tracking para actualizarlo)
                    var articulo = await _context.Articulos.FindAsync(item.ArticuloId);

                    if (articulo == null)
                        throw new Exception($"El artículo con ID {item.ArticuloId} no fue encontrado.");

                    // 3. Validación crítica de Stock
                    if (articulo.StockActual < item.Cantidad)
                    {
                        throw new Exception($"Stock insuficiente para '{articulo.Nombre}'. Disponible: {articulo.StockActual}, Solicitado: {item.Cantidad}");
                    }

                    // 4. Descontar stock
                    articulo.StockActual -= item.Cantidad;
                }

                // 5. Guardar cabecera e ítems
                _context.Remitos.Add(remito);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return remito;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Re-lanzamos la excepción para que el controlador la capture en el try/catch
            }
        }

        public async Task UpdateAsync(Remito remito)
        {
            // 1. Buscamos el original con sus items
            var existingRemito = await _context.Remitos
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == remito.Id);

            if (existingRemito == null) throw new Exception("Remito no encontrado");

            // 2. Actualizamos campos básicos
            existingRemito.NumeroRemito = remito.NumeroRemito;
            existingRemito.FechaEmision = remito.FechaEmision;
            existingRemito.Estado = remito.Estado;
            existingRemito.Observaciones = remito.Observaciones;

            // AQUÍ EL ERROR: Asegurate de asignar el ID, no el objeto completo
            existingRemito.ClienteId = remito.ClienteId;
            existingRemito.SedeId = remito.SedeId;

            // 3. Manejo de Items (Limpiar y Re-agregar es lo más seguro en ERPs)
            _context.RemitoItems.RemoveRange(existingRemito.Items);

            foreach (var item in remito.Items)
            {
                existingRemito.Items.Add(new RemitoItem
                {
                    ArticuloId = item.ArticuloId,
                    Cantidad = item.Cantidad,
                    Notas = item.Notas
                });
            }

            await _context.SaveChangesAsync();
        }


        public async Task UpdateEstadoAsync(int id, string nuevoEstado)
        {
            // Solo traemos el remito, sin los items (ahorramos memoria y tiempo)
            var remito = await _context.Remitos.FindAsync(id);

            if (remito == null) throw new Exception("Remito no encontrado");

            remito.Estado = nuevoEstado;

            // Entity Framework es inteligente: solo generará el SQL para la columna Estado
            await _context.SaveChangesAsync();
        }

        public async Task AnularAsync(int id)
        {
            var remito = await _context.Remitos.FindAsync(id);
            if (remito != null)
            {
                remito.Estado = "Anulado";
                remito.FechaActualizacion = DateTime.Now;
                // Aquí podrías decidir si devolvés el stock al anular, depende de tu lógica de negocio
                await _context.SaveChangesAsync();
            }
        }


    }

}
