using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PerfilesController : ControllerBase
{
    private readonly IPerfilesRepository _perfilesRepository;

    public PerfilesController(IPerfilesRepository perfilesRepository)
    {
        _perfilesRepository = perfilesRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var perfiles = await _perfilesRepository.GetAllAsync();
        return Ok(perfiles);
    }

    // Menú dinámico para el Sidebar
    [HttpGet("{idPerfil}/menu")]
    public async Task<ActionResult<List<MenuNodeDto>>> GetMenu(int idPerfil)
    {
        var menuesEntidad = await _perfilesRepository.GetMenuDinamico(idPerfil);

        var response = menuesEntidad.Select(m => new MenuNodeDto
        {
            IdMenu = m.IdMenu,
            Nombre = m.Nombre,
            Ruta = m.Ruta,
            Icono = m.Icono,
            SubMenues = m.SubMenues.Select(sm => new SubMenuDto
            {
                Nombre = sm.Nombre,
                Ruta = sm.Ruta
            }).ToList()
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{idPerfil}/permisos")]
    public async Task<IActionResult> GetPermisos(int idPerfil)
    {
        // Ahora esta lista contendrá TODOS los menús del sistema mapeados a la entidad MenuesXPerfil
        var permisosRaw = await _perfilesRepository.GetPermisosPorPerfil(idPerfil);

        var todosLosDtos = permisosRaw.Select(p => new MenuPermisoDto
        {
            IdMenu = p.idMenu,
            Nombre = p.Menu?.Nombre ?? "Sin Nombre",
            Ruta = p.Menu?.Ruta,
            Orden = p.Menu?.Orden ?? 0,
            Ver = p.PermisoVer,
            Crear = p.PermisoCrear,
            Editar = p.PermisoEditar,
            Eliminar = p.PermisoEliminar,
            IdPadre = p.Menu?.IdPadre
        }).ToList();

        // El resto de tu lógica de jerarquía se mantiene igual...
        var response = todosLosDtos
            .Where(p => p.IdPadre == null)
            .OrderBy(p => p.Orden)
            .Select(p => {
                p.SubPermisos = todosLosDtos
                    .Where(h => h.IdPadre == p.IdMenu)
                    .OrderBy(h => h.Orden)
                    .ToList();
                return p;
            })
            .ToList();

        return Ok(response);
    }

    [HttpPut("permisos")]
    public async Task<IActionResult> UpdatePermisos([FromBody] List<MenuesXPerfil> permisos)
    {
        try
        {
            await _perfilesRepository.UpdatePermisosAsync(permisos);
            return Ok(new { message = "Permisos actualizados correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}