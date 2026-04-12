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
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                // Buscamos por número de remito o nombre del cliente
                query = query.Where(r => r.NumeroRemito.Contains(search) ||
                                         r.Cliente.DNI.Contains(search));
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
                    .ThenInclude(i => i.Articulo) // Traemos el nombre del artículo para cada renglón
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Remito> CreateAsync(Remito remito)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                remito.CreatedAt = DateTime.Now;
                remito.Estado = remito.Estado ?? "Emitido";

                _context.Remitos.Add(remito);

                // Descontamos el stock de cada artículo incluido en el remito
                foreach (var item in remito.Items)
                {
                    var articulo = await _context.Articulos.FindAsync(item.ArticuloId);

                    if (articulo != null)
                    {
                        // Restamos la cantidad enviada del stock actual
                        articulo.StockActual -= item.Cantidad;
                        articulo.UpdatedAt = DateTime.Now;

                        _context.Articulos.Update(articulo);
                    }
                    else
                    {
                        throw new Exception($"El artículo con ID {item.ArticuloId} no existe.");
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return remito;
            }
            catch (Exception ex)
            {
                // Si algo falla (ej: error de red, id inexistente), deshacemos los cambios
                await transaction.RollbackAsync();
                throw new Exception("Error al procesar el remito y actualizar el stock.", ex);
            }
        }


        public async Task UpdateAsync(Remito remito)
        {
            // Cargar el remito existente CON sus ítems desde la DB (con tracking)
            var remitoEnDb = await _context.Remitos
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == remito.Id);

            if (remitoEnDb == null) return;

            // Actualizar campos escalares del padre
            remitoEnDb.Estado = remito.Estado;
            remitoEnDb.Observaciones = remito.Observaciones;
            remitoEnDb.FechaEmision = remito.FechaEmision;
            remitoEnDb.UpdatedAt = DateTime.Now;

            // (requiere que RemitoItem tenga cascade delete configurado)
            _context.RemoveRange(remitoEnDb.Items);
            remitoEnDb.Items = remito.Items.Select(i => new RemitoItem
            {
                RemitoId = remito.Id,
                ArticuloId = i.ArticuloId,
                Cantidad = i.Cantidad,
                Notas = i.Notas
            }).ToList();

            await _context.SaveChangesAsync();
        }

        public async Task AnularAsync(int id)
        {
            var remito = await _context.Remitos.FindAsync(id);
            if (remito != null)
            {
                remito.Estado = "Anulado";
                remito.UpdatedAt = DateTime.Now;
                // Aquí podrías decidir si devolvés el stock al anular, depende de tu lógica de negocio
                await _context.SaveChangesAsync();
            }
        }


    }

}
