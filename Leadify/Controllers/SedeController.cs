using Leadify.Application.DTOs;
using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedesController : ControllerBase
    {
        private readonly ISedeRepository _sedeRepo;

        public SedesController(ISedeRepository sedeRepo)
        {
            _sedeRepo = sedeRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            // El repo devuelve Entidades (Sede con .Include(s => s.Cliente))
            var pagedEntities = await _sedeRepo.GetPagedAsync(page, size, search);

            // Mapeamos a DTOs de salida
            var dtos = pagedEntities.Items.Select(s => new SedeResponseDto
            {
                Id = s.Id,
                ClienteId = s.ClienteId,
                ClienteNombre = s.Cliente?.Nombre ?? "Sin Cliente",
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Provincia = s.Provincia,
                CodigoPostal = s.CodigoPostal,
                ContactoNombre = s.ContactoNombre,
                ContactoTelefono = s.ContactoTelefono,
                Activo = s.Activo
            }).ToList();

            return Ok(new PagedResult<SedeResponseDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageIndex = pagedEntities.PageIndex,
                PageSize = pagedEntities.PageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SedeDto dto)
        {
            // Mapeo manual a la entidad Sede
            var nuevaSede = new Sede
            {
                ClienteId = dto.ClienteId,
                Nombre = dto.Nombre,
                Direccion = dto.Direccion,
                Localidad = dto.Localidad ?? string.Empty,
                Provincia = dto.Provincia ?? string.Empty,
                CodigoPostal = dto.CodigoPostal ?? string.Empty,
                ContactoNombre = dto.ContactoNombre ?? string.Empty,
                ContactoTelefono = dto.ContactoTelefono ?? string.Empty,
                Activo = true
            };

            await _sedeRepo.CreateAsync(nuevaSede);
            return CreatedAtAction(nameof(GetPaged), new { id = nuevaSede.Id }, nuevaSede);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _sedeRepo.GetByIdAsync(id);

            if (s == null) return NotFound();

            // Mapeamos a la misma estructura que espera el frontend
            var dto = new SedeResponseDto
            {
                Id = s.Id,
                ClienteId = s.ClienteId,
                ClienteNombre = s.Cliente?.Nombre ?? "Sin Cliente",
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Localidad = s.Localidad,
                Provincia = s.Provincia,
                CodigoPostal = s.CodigoPostal,
                ContactoNombre = s.ContactoNombre,
                ContactoTelefono = s.ContactoTelefono,
                Activo = s.Activo
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SedeDto dto)
        {
            var sedeExistente = await _sedeRepo.GetByIdAsync(id);
            if (sedeExistente == null) return NotFound();

            // Campos faltantes en tu snippet:
            sedeExistente.ClienteId = dto.ClienteId;
            sedeExistente.CodigoPostal = dto.CodigoPostal ?? string.Empty;

            // Campos que ya tenías:
            sedeExistente.Nombre = dto.Nombre;
            sedeExistente.Direccion = dto.Direccion;
            sedeExistente.Localidad = dto.Localidad ?? string.Empty;
            sedeExistente.Provincia = dto.Provincia ?? string.Empty;
            sedeExistente.ContactoNombre = dto.ContactoNombre ?? string.Empty;
            sedeExistente.ContactoTelefono = dto.ContactoTelefono ?? string.Empty;
            sedeExistente.Activo = dto.Activo;

            await _sedeRepo.UpdateAsync(sedeExistente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _sedeRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}