using Leadify.Application.DTOs;
using Leadify.Application.Interfaces;
using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Hosting;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorRepository _repo;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;

        public ProveedoresController(IProveedorRepository repo, IFileService fileService, IWebHostEnvironment env)
        {
            _repo = repo;
            _fileService = fileService;
            _env = env;
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


        //------MANEJO DE ARCHIVO ADJUNTO--------------------

        // GET: api/proveedores/{id}/archivos
        [HttpGet("{id}/archivos")]
        public async Task<IActionResult> GetArchivos(int id)
        {
            var proveedor = await _repo.GetByIdAsync(id);
            if (proveedor == null) return NotFound();

            var archivos = await _repo.GetArchivosByProveedorId(id);

       
            return Ok(archivos);
        }

        // POST: api/proveedores/{id}/archivos
        [HttpPost("{id}/archivos")]
        public async Task<IActionResult> SubirAdjunto(int id, IFormFile file)
        {
            var proveedor = await _repo.GetByIdAsync(id);
            if (proveedor == null) return NotFound();

            try
            {
                // El servicio se encarga de crear la carpeta y guardar (ahora en wwwroot/adjuntos)
                string subCarpeta = $"proveedores/{id}";
                string urlRelativa = await _fileService.GuardarArchivo(file, subCarpeta);

                var archivo = new ProveedorArchivo
                {
                    ProveedorId = id,
                    NombreOriginal = file.FileName,
                    UrlRelativa = urlRelativa,
                    FechaSubida = DateTime.Now
                };

                await _repo.SaveArchivoAsync(archivo);
                return Ok(archivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al guardar el archivo: {ex.Message}");
            }
        }

        [HttpGet("descargar-archivo/{archivoId}")]
        public async Task<IActionResult> DescargarArchivo(int archivoId, [FromQuery] bool? verEnNavegador = false)
        {
            var archivo = await _repo.GetArchivoById(archivoId);
            if (archivo == null) return NotFound("Archivo no encontrado.");

            
            var path = Path.Combine(_env.WebRootPath, archivo.UrlRelativa.TrimStart('/'));

            if (!System.IO.File.Exists(path)) return NotFound("Archivo físico no encontrado.");

            var provider = new FileExtensionContentTypeProvider();
            provider.TryGetContentType(path, out var contentType);
            contentType ??= "application/octet-stream";

            var disposition = (verEnNavegador == true) ? "inline" : "attachment";
            Response.Headers.Add("Content-Disposition", $"{disposition}; filename=\"{archivo.NombreOriginal}\"");

            return PhysicalFile(path, contentType);
        }

        // DELETE: api/proveedores/archivos/{archivoId}
        [HttpDelete("archivos/{archivoId}")]
        public async Task<IActionResult> EliminarArchivo(int archivoId)
        {
            var archivo = await _repo.GetArchivoById(archivoId);
            if (archivo == null) return NotFound();

            archivo.Activo = false;

            await _repo.UpdateArchivo(archivo); // Necesitarías crear este método en tu repo

            return NoContent();
        }
    }
}