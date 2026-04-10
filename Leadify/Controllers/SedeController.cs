using Microsoft.AspNetCore.Mvc;
using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;

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
            return Ok(await _sedeRepo.GetPagedAsync(page, size, search));
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SedeDto dto)
        {
            var sedeExistente = await _sedeRepo.GetByIdAsync(id);
            if (sedeExistente == null) return NotFound();

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