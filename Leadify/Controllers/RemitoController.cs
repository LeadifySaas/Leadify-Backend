using Leadify.Application.DTOs;
using Leadify.Application.Interfaces;
using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemitoController : ControllerBase
    {
        private readonly IRemitoRepository _remitoRepo;
        private readonly IRemitoReportService _reportService; 
        public RemitoController(IRemitoRepository remitoRepo, IRemitoReportService reportService)
        {
            _remitoRepo = remitoRepo;
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            // El repo devuelve Entidades (Domain)
            var pagedEntities = await _remitoRepo.GetPagedAsync(page, size, search);

            // Mapeamos a DTOs (Application) justo antes de salir por el cable
            var dtos = pagedEntities.Items.Select(r => new RemitoResponseDto
            {
                Id = r.Id,
                NumeroRemito = r.NumeroRemito,
                ClienteNombre = r.Cliente?.Nombre ?? "S/D",
                SedeNombre = r.Sede?.Nombre ?? "S/D",
                FechaEmision = r.FechaEmision,
                Estado = r.Estado,
                Observaciones = r.Observaciones,
                CreadoPor = r.CreadoPorUsuario != null
                ? r.CreadoPorUsuario.Nombre
                : "Sistema"

            }).ToList();

            return Ok(new PagedResult<RemitoResponseDto>
            {
                Items = dtos,
                TotalCount = pagedEntities.TotalCount,
                PageIndex = pagedEntities.PageIndex,
                PageSize = pagedEntities.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Usamos tu repositorio como corresponde
            var remito = await _remitoRepo.GetByIdAsync(id);

            if (remito == null) return NotFound("El remito solicitado no existe.");

            // Mapeamos a un objeto anónimo (o un DTO) para romper la circularidad
            // Esto evita que el serializador intente entrar en Cliente -> Remitos -> Cliente
            var response = new
            {
                remito.Id,
                remito.NumeroRemito,
                remito.FechaEmision,
                remito.Estado,
                remito.Observaciones,
                Cliente = new
                {
                    remito.Cliente?.Id,
                    remito.Cliente?.Nombre
                },
                Sede = new
                {
                    remito.Sede?.Id,
                    remito.Sede?.Nombre,
                    remito.Sede?.Localidad
                },
                Items = remito.Items.Select(i => new
                {
                    i.ArticuloId,
                    ArticuloNombre = i.Articulo?.Nombre,
                    i.Cantidad,
                    i.Notas
                }).ToList()
            };

            return Ok(response);
        }

        private int ObtenerUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new Exception("Usuario no autenticado");

            return int.Parse(userIdClaim.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RemitoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var usuarioId = ObtenerUsuarioId(); 

                var nuevoRemito = new Remito
                {
                    NumeroRemito = dto.NumeroRemito,
                    ClienteId = dto.ClienteId,
                    SedeId = dto.SedeId,
                    FechaEmision = dto.FechaEmision,
                    Estado = dto.Estado ?? "Pendiente",
                    Observaciones = dto.Observaciones,

                    CreadoPorUsuarioId = usuarioId,

                    Items = dto.Items.Select(i => new RemitoItem
                    {
                        ArticuloId = i.ArticuloId,
                        Cantidad = i.Cantidad,
                        Notas = i.Notas
                    }).ToList()
                };

                var creado = await _remitoRepo.CreateAsync(nuevoRemito);

                return Ok(new
                {
                    id = creado.Id,
                    message = "Remito creado correctamente",
                    numeroRemito = creado.NumeroRemito
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error de validación", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RemitoDto dto)
        {
            var remitoUpdate = new Remito
            {
                Id = id,
                NumeroRemito = dto.NumeroRemito,
                ClienteId = dto.ClienteId,   // ✅ IMPORTANTE
                SedeId = dto.SedeId,         // ✅ IMPORTANTE
                Estado = dto.Estado,
                Observaciones = dto.Observaciones,
                FechaEmision = dto.FechaEmision,
                Items = dto.Items.Select(i => new RemitoItem
                {
                    ArticuloId = i.ArticuloId,
                    Cantidad = i.Cantidad,
                    Notas = i.Notas
                }).ToList()
            };

            try
            {
                await _remitoRepo.UpdateAsync(remitoUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { detail = ex.InnerException?.Message ?? ex.Message });
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Anular(int id)
        {
            var remito = await _remitoRepo.GetByIdAsync(id);
            if (remito == null) return NotFound();

            await _remitoRepo.AnularAsync(id);
            return NoContent();
        }


        // Agrega estos métodos a tu RemitoController.cs

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] string nuevoEstado)
        {
            try
            {
                // Importante: si el body es solo un string, a veces llega con comillas
                // dependiendo de cómo lo mande el frontend.
                var estadoLimpio = nuevoEstado.Replace("\"", "");

                await _remitoRepo.UpdateEstadoAsync(id, estadoLimpio);
                return Ok(new { message = "Estado actualizado con éxito" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GetPdf(int id)
        {
            // Importante: GetByIdAsync debe incluir (.Include) Cliente, Sede e Items 
            // para que el PDF no salga vacío o con errores.
            var remito = await _remitoRepo.GetByIdAsync(id);

            if (remito == null) return NotFound(new { message = "Remito no encontrado" });

            try
            {
                byte[] pdfBytes = _reportService.GenerarPdf(remito);

                // "application/pdf" le dice al navegador que lo abra con su visor interno
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el PDF", details = ex.Message });
            }
        }

        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarExcel([FromQuery] string? search)
        {
            try
            {
                // Necesitamos un método en el repo que traiga la lista (puedes reutilizar el de búsqueda)
                // pero que devuelva un IEnumerable sin paginar para el reporte.
                var remitosPaged = await _remitoRepo.GetPagedAsync(1, 1000, search);
                var remitos = remitosPaged.Items;

                byte[] excelBytes = _reportService.GenerarExcel(remitos);

                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Listado_Remitos.xlsx"
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al generar el Excel", details = ex.Message });
            }
        }
    }
}