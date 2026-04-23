using Microsoft.AspNetCore.Mvc;
using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosInternoController : ControllerBase
    {
        private readonly IArticuloInternoRepository _articuloRepo;

        public ArticulosInternoController(IArticuloInternoRepository articuloRepo)
        {
            _articuloRepo = articuloRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            var result = await _articuloRepo.GetPagedAsync(page, size, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var articulo = await _articuloRepo.GetByIdAsync(id);
            if (articulo == null) return NotFound();
            return Ok(articulo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticuloInternoDto dto)
        {
            if (await _articuloRepo.ExisteCodigoAsync(dto.Codigo))
                return BadRequest("El código de artículo ya existe.");

            var articulo = new ArticulosInternos
            {
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                UnidadMedida = dto.UnidadMedida,
                PrecioVenta = dto.PrecioVenta,
                StockActual = dto.StockActual
            };

            await _articuloRepo.CreateAsync(articulo);
            return CreatedAtAction(nameof(GetById), new { id = articulo.Id }, articulo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticuloInternoDto dto)
        {
            var articuloExistente = await _articuloRepo.GetByIdAsync(id);
            if (articuloExistente == null) return NotFound();

            articuloExistente.Nombre = dto.Nombre;
            articuloExistente.Descripcion = dto.Descripcion;
            articuloExistente.UnidadMedida = dto.UnidadMedida;
            articuloExistente.PrecioVenta = dto.PrecioVenta;
            articuloExistente.StockActual = dto.StockActual;
            articuloExistente.Activo = dto.Activo;

            await _articuloRepo.UpdateAsync(articuloExistente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articuloRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}