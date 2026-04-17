using Leadify.Application.DTOs;
using Leadify.Application.Interfaces;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;
using Leadify.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Leadify.Infrastructure.Data;

namespace Leadify.API.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class EmpresaController : ControllerBase
    {
       
        private readonly IEmpresaRepository _empresaRepo;

        public EmpresaController(IEmpresaRepository empresaRepo)
        {
            _empresaRepo = empresaRepo;
        }

        // GET: api/empresas?page=1&size=25&search=...
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            // Busca por RazonSocial o CUIT según el repositorio
            return Ok(await _empresaRepo.GetPagedAsync(page, size, search));
        }

        // GET: api/empresas/cliente/5
        // Útil para listar las empresas de un cliente específico en su ficha
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByClienteId(int clienteId)
        {
            var empresas = await _empresaRepo.GetByClienteIdAsync(clienteId);
            return Ok(empresas);
        }

        // GET: api/empresas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var empresa = await _empresaRepo.GetByIdAsync(id);
            if (empresa == null) return NotFound();
            return Ok(empresa);
        }

        // POST: api/empresas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpresaDto dto)
        {
            // Validamos que el CUIT no exista
            if (!string.IsNullOrEmpty(dto.Cuit) && await _empresaRepo.ExisteCuitAsync(dto.Cuit))
            {
                return BadRequest("El CUIT de la empresa ya se encuentra registrado.");
            }

            if (!string.IsNullOrEmpty(dto.EmailFacturacion) && await _empresaRepo.ExisteEmailAsync(dto.EmailFacturacion))
            {
                return BadRequest(new { message = "El email ya se encuentra registrado." });
            }

            var empresa = new Empresa
            {
                ClienteId = dto.ClienteId,
                RazonSocial = dto.RazonSocial,
                Cuit = dto.Cuit,
                DireccionFiscal = dto.DireccionFiscal,
                EmailFacturacion = dto.EmailFacturacion,
                CondicionIva = dto.CondicionIva,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            await _empresaRepo.CreateAsync(empresa);
            return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
        }

        // PUT: api/empresas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmpresaDto dto)
        {
            var empresaExistente = await _empresaRepo.GetByIdAsync(id);
            if (empresaExistente == null) return NotFound();

            // Validación de CUIT duplicado al editar (ignorando el propio)
            if (!string.IsNullOrEmpty(dto.Cuit) && dto.Cuit != empresaExistente.Cuit)
            {
                if (await _empresaRepo.ExisteCuitAsync(dto.Cuit))
                    return BadRequest("El nuevo CUIT ya está siendo usado por otra empresa.");
            }

            if (!string.IsNullOrEmpty(dto.EmailFacturacion) && dto.EmailFacturacion.ToLower() != empresaExistente.EmailFacturacion?.ToLower())
            {
                if (await _empresaRepo.ExisteEmailAsync(dto.EmailFacturacion))
                    return BadRequest(new { message = "El nuevo email ya está siendo usado por otra empresa." });
            }

            empresaExistente.ClienteId = dto.ClienteId; // Permite el cambio de dueño que hablamos
            empresaExistente.RazonSocial = dto.RazonSocial;
            empresaExistente.Cuit = dto.Cuit;
            empresaExistente.DireccionFiscal = dto.DireccionFiscal;
            empresaExistente.EmailFacturacion = dto.EmailFacturacion;
            empresaExistente.CondicionIva = dto.CondicionIva;
            empresaExistente.Activo = dto.Activo;

            await _empresaRepo.UpdateAsync(empresaExistente);
            return NoContent();
        }

        // DELETE: api/empresas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // El repositorio ya aplica el borrado lógico (Activo = false)
            await _empresaRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}