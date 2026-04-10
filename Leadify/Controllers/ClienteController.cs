using Microsoft.AspNetCore.Mvc;
using Leadify.Application.DTOs;
using Leadify.Domain.Entities;
using Leadify.Domain.Interfaces;

namespace Leadify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepo;

        public ClientesController(IClienteRepository clienteRepo)
        {
            _clienteRepo = clienteRepo;
        }

        // GET: api/clientes?page=1&size=10&search=...
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            return Ok(await _clienteRepo.GetPagedAsync(page, size, search));
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _clienteRepo.GetByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteDto dto)
        {
            if (await _clienteRepo.ExisteCuitAsync(dto.CUIT)) return BadRequest("CUIT Duplicado");

            var cliente = new Cliente
            {
                RazonSocial = dto.RazonSocial,
                CUIT = dto.CUIT,
                Email = dto.Email,
                Telefono = dto.Telefono,
                CondicionIVA = dto.CondicionIVA,
                Activo = true,
                CreatedAt = DateTime.Now 
            };

            await _clienteRepo.CreateAsync(cliente);
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteDto dto)
        {
            var clienteExistente = await _clienteRepo.GetByIdAsync(id);
            if (clienteExistente == null) return NotFound();

            clienteExistente.RazonSocial = dto.RazonSocial;
            clienteExistente.CUIT = dto.CUIT;
            clienteExistente.Email = dto.Email;
            clienteExistente.Telefono = dto.Telefono;
            clienteExistente.CondicionIVA = dto.CondicionIVA;
            clienteExistente.Activo = dto.Activo;


            await _clienteRepo.UpdateAsync(clienteExistente);
            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _clienteRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}