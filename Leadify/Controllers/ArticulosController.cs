using Leadify.Application.DTOs;
using Leadify.Application.Interfaces;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : ControllerBase
    {
        private readonly IArticuloRepository _articuloRepo;
        private readonly IFileService _fileService;

        public ArticulosController(IArticuloRepository articuloRepo, IFileService fileService)
        {
            _articuloRepo = articuloRepo;
            _fileService = fileService;
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
        public async Task<IActionResult> Create([FromBody] ArticuloDto dto)
        {
            if (await _articuloRepo.ExisteCodigoAsync(dto.Codigo))
                return BadRequest("El código de artículo ya existe.");

            var articulo = new Articulo
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
        public async Task<IActionResult> Update(int id, [FromBody] ArticuloDto dto)
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

        // POST: api/articulos/{id}/imagen
        [HttpPost("{idArticulo}/imagen")]
        public async Task<IActionResult> SubirImagen(int idArticulo, IFormFile file)
        {
            var articulo = await _articuloRepo.GetByIdAsync(idArticulo);
            if (articulo == null) return NotFound("Artículo no encontrado.");

            try
            {
                if (!string.IsNullOrEmpty(articulo.ImagenUrl))
                {
                    await _fileService.EliminarArchivo(articulo.ImagenUrl);
                }
                string urlRelativa = await _fileService.GuardarArchivo(file, "articulos");

                articulo.ImagenUrl = urlRelativa;
                await _articuloRepo.UpdateAsync(articulo);

                return Ok(new { ImagenUrl = articulo.ImagenUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar la imagen: {ex.Message}");
            }
        }

        // DELETE: api/articulos/{id}/imagen
        [HttpDelete("{id}/imagen")]
        public async Task<IActionResult> EliminarImagen(int id)
        {
            var articulo = await _articuloRepo.GetByIdAsync(id);
            if (articulo == null) return NotFound();

            if (!string.IsNullOrEmpty(articulo.ImagenUrl))
            {
                await _fileService.EliminarArchivo(articulo.ImagenUrl);

                articulo.ImagenUrl = null;
                await _articuloRepo.UpdateAsync(articulo);
            }

            return NoContent();
        }
    }
}