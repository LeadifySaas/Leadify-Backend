using Leadify.Application.DTOs;
using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorRepository _repo;

        public ProveedoresController(IProveedorRepository repo)
        {
            _repo = repo;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            var pagedResult = await _repo.GetPagedAsync(page, size, search);

            // Mapeamos manualmente a una lista de DTOs antes de devolverla
            var dtos = pagedResult.Items.Select(p => new ProveedorDto
            {
                Id = p.Id,
                RazonSocial = p.RazonSocial,
                Cuit = p.Cuit,
                Email = p.Email,
                Telefono = p.Telefono,
                Rubro = p.Rubro,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            }).ToList();

            // Devolvemos el mismo formato que espera tu frontend pero con los DTOs
            return Ok(new
            {
                Items = dtos,
                pagedResult.TotalCount,
                pagedResult.PageIndex,
                pagedResult.PageSize
            });
        }

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return NotFound();

            // Mapeo manual a DTO
            var dto = new ProveedorDto
            {
                Id = p.Id,
                RazonSocial = p.RazonSocial,
                Cuit = p.Cuit,
                Email = p.Email,
                Telefono = p.Telefono,
                Rubro = p.Rubro,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            };

            return Ok(dto);
        }

        // POST: api/proveedores
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProveedorDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Cuit) && await _repo.ExisteCuitAsync(dto.Cuit))
            {
                return BadRequest("El CUIT del proveedor ya se encuentra registrado.");
            }

            if (!string.IsNullOrEmpty(dto.Email) && await _repo.ExisteEmailAsync(dto.Email))
            {
                return BadRequest(new { message = "El email ya se encuentra registrado." });
            }

            var proveedor = new Proveedor
            {
                RazonSocial = dto.RazonSocial,
                Cuit = dto.Cuit,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Rubro = dto.Rubro,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            await _repo.CreateAsync(proveedor);

            // Creamos un DTO para devolver como respuesta
            var nuevoDto = new ProveedorDto
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                Cuit = proveedor.Cuit,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono,
                Rubro = proveedor.Rubro,
                Activo = proveedor.Activo,
                FechaCreacion = proveedor.FechaCreacion
            };

            // Retornamos el DTO
            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, nuevoDto);
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProveedorDto dto)
        {
            var pExistente = await _repo.GetByIdAsync(id);
            if (pExistente == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.Cuit) && dto.Cuit != pExistente.Cuit)
            {
                if (await _repo.ExisteCuitAsync(dto.Cuit))
                    return BadRequest("El nuevo CUIT ya está siendo usado por otro proveedor.");
            }

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email.ToLower() != pExistente.Email?.ToLower())
            {
                if (await _repo.ExisteEmailAsync(dto.Email))
                    return BadRequest(new { message = "El nuevo email ya está siendo usado por otro proveedor." });
            }

            // Asignación manual
            pExistente.RazonSocial = dto.RazonSocial;
            pExistente.Cuit = dto.Cuit;
            pExistente.Email = dto.Email;
            pExistente.Telefono = dto.Telefono;
            pExistente.Rubro = dto.Rubro;
            pExistente.Activo = dto.Activo;

            await _repo.UpdateAsync(pExistente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }
}