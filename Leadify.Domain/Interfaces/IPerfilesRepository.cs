using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Leadify.Domain.Interfaces
{
    public interface IPerfilesRepository
    {
        Task<List<MenuesXPerfil>> GetPermisosPorPerfil(int idPerfil);

        // Probablemente necesites estos para el modal de React que vimos
        Task<Perfiles> GetByIdAsync(int id);
        Task<List<Perfiles>> GetAllAsync();
        Task UpdatePermisosAsync(List<MenuesXPerfil> permisos);

        Task<List<Menues>> GetMenuDinamico(int idPerfil);
    }
}
