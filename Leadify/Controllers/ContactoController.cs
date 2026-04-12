using Microsoft.AspNetCore.Mvc;
using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactosController : ControllerBase
    {
        private readonly IContactoRepository _contactoRepo;

        public ContactosController(IContactoRepository contactoRepo)
        {
            _contactoRepo = contactoRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
            => Ok(await _contactoRepo.GetPagedAsync(page, size, search));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contacto = await _contactoRepo.GetByIdAsync(id);
            return contacto == null ? NotFound() : Ok(contacto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContactoDto dto)
        {
            var contacto = new Contacto
            {
                ClienteId = dto.ClienteId,
                EmpresaId = dto.EmpresaId,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Puesto = dto.Puesto,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Observaciones = dto.Observaciones
            };
            await _contactoRepo.CreateAsync(contacto);
            return CreatedAtAction(nameof(GetById), new { id = contacto.Id }, contacto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContactoDto dto)
        {
            var existente = await _contactoRepo.GetByIdAsync(id);
            if (existente == null) return NotFound();

            existente.Nombre = dto.Nombre;
            existente.Apellido = dto.Apellido;
            existente.Puesto = dto.Puesto;
            existente.Telefono = dto.Telefono;
            existente.Email = dto.Email;
            existente.Observaciones = dto.Observaciones;
            existente.Activo = dto.Activo;

            await _contactoRepo.UpdateAsync(existente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactoRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}