using Microsoft.AspNetCore.Mvc;
using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemitoController : ControllerBase
    {
        private readonly IRemitoRepository _remitoRepo;

        public RemitoController(IRemitoRepository remitoRepo)
        {
            _remitoRepo = remitoRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            var result = await _remitoRepo.GetPagedAsync(page, size, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var remito = await _remitoRepo.GetByIdAsync(id);
            if (remito == null) return NotFound("El remito solicitado no existe.");
            return Ok(remito);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RemitoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var nuevoRemito = new Remito
                {
                    NumeroRemito = dto.NumeroRemito,
                    ClienteId = dto.ClienteId,
                    SedeId = dto.SedeId,
                    FechaEmision = dto.FechaEmision,
                    Estado = "Pendiente", // O el que definas por defecto
                    Observaciones = dto.Observaciones,
                    Items = dto.Items.Select(i => new RemitoItem
                    {
                        ArticuloId = i.ArticuloId,
                        Cantidad = i.Cantidad,
                        Notas = i.Notas
                    }).ToList()
                };

                // El Repo debería encargarse de la transacción de Stock
                var creado = await _remitoRepo.CreateAsync(nuevoRemito);

                // Retornamos el DTO de respuesta que el frontend usará para actualizar la lista
                var response = await _remitoRepo.GetByIdAsync(creado.Id);
                return CreatedAtAction(nameof(GetById), new { id = creado.Id }, response);
            }
            catch (Exception ex)
            {
                // Importante para el frontend manejar este error (ej: "No hay stock suficiente")
                return BadRequest(new { message = "Error al procesar el remito", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RemitoDto dto)
        {
            var remitoExistente = await _remitoRepo.GetByIdAsync(id);
            if (remitoExistente == null) return NotFound();

            remitoExistente.Estado = dto.Estado;
            remitoExistente.Observaciones = dto.Observaciones;
            remitoExistente.FechaEmision = dto.FechaEmision;

            // Reemplazar ítems: limpiar los viejos y cargar los nuevos
            remitoExistente.Items.Clear();
            remitoExistente.Items = dto.Items.Select(i => new RemitoItem
            {
                RemitoId = id,
                ArticuloId = i.ArticuloId,
                Cantidad = i.Cantidad,
                Notas = i.Notas
            }).ToList();

            await _remitoRepo.UpdateAsync(remitoExistente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Anular(int id)
        {
            var remito = await _remitoRepo.GetByIdAsync(id);
            if (remito == null) return NotFound();

            await _remitoRepo.AnularAsync(id);
            return NoContent();
        }
    }
}