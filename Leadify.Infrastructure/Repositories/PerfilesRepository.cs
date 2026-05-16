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
    public class PerfilesRepository : IPerfilesRepository
    {
        private readonly ApplicationDbContext _context;

        public PerfilesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuesXPerfil>> GetPermisosPorPerfil(int idPerfil)
        {
            var todosLosMenues = await _context.Menues
                .Where(m => m.Estado == true)
                .OrderBy(m => m.Orden)
                .ToListAsync();

            var permisosExistentes = await _context.MenuesXPerfil
                .Where(x => x.idPerfil == idPerfil)
                .ToListAsync();

            var resultado = todosLosMenues.Select(menu =>
            {
                var permiso = permisosExistentes.FirstOrDefault(x => x.idMenu == menu.IdMenu);

                return new MenuesXPerfil
                {
                    idPerfil = idPerfil,
                    idMenu = menu.IdMenu,
                    Menu = menu, // Importante para que el DTO tenga el Nombre/Ruta
                    PermisoVer = permiso?.PermisoVer ?? false,
                    PermisoCrear = permiso?.PermisoCrear ?? false,
                    PermisoEditar = permiso?.PermisoEditar ?? false,
                    PermisoEliminar = permiso?.PermisoEliminar ?? false
                };
            }).ToList();

            return resultado;
        }

        // 2. Obtiene todos los perfiles (para la tabla principal de la imagen 1)
        public async Task<List<Perfiles>> GetAllAsync()
        {
            return await _context.Perfiles
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        // 3. Obtiene un perfil por su ID
        public async Task<Perfiles> GetByIdAsync(int id)
        {
            return await _context.Perfiles.FirstOrDefaultAsync(x => x.IdPerfil == id);
        }

        // 4. Guarda los cambios de la matriz de permisos (Botón "Guardar" del modal)
        public async Task UpdatePermisosAsync(List<MenuesXPerfil> permisos)
        {
            foreach (var p in permisos)
            {
                var existente = await _context.MenuesXPerfil
                    .FirstOrDefaultAsync(x => x.idMenu == p.idMenu && x.idPerfil == p.idPerfil);

                if (existente != null)
                {
                    // UPDATE
                    existente.PermisoVer = p.PermisoVer;
                    existente.PermisoCrear = p.PermisoCrear;
                    existente.PermisoEditar = p.PermisoEditar;
                    existente.PermisoEliminar = p.PermisoEliminar;
                }
                else
                {
                    // INSERT
                    if (p.PermisoVer || p.PermisoCrear || p.PermisoEditar || p.PermisoEliminar)
                    {
                        _context.MenuesXPerfil.Add(p);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Menues>> GetMenuDinamico(int idPerfil)
        {
            // Obtenemos los IDs de los menues que este perfil tiene permitido ver
            var menuesPermitidosIds = await _context.MenuesXPerfil
                .Where(mxp => mxp.idPerfil == idPerfil && mxp.PermisoVer)
                .Select(mxp => mxp.idMenu)
                .ToListAsync();

            // Traemos los menues que son padres (IdPadre == null) y están en la lista permitida
            var menuTree = await _context.Menues
                .Include(m => m.SubMenues)
                .Where(m => m.IdPadre == null && m.Estado == true && menuesPermitidosIds.Contains(m.IdMenu))
                .OrderBy(m => m.Orden)
                .ToListAsync();

            // Filtramos los SubMenues de cada padre para que solo aparezcan los permitidos
            foreach (var padre in menuTree)
            {
                padre.SubMenues = padre.SubMenues
                    .Where(sm => sm.Estado == true && menuesPermitidosIds.Contains(sm.IdMenu))
                    .OrderBy(sm => sm.Orden)
                    .ToList();
            }

            return menuTree;
        }
    }
}
