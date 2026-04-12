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

        // GET: api/clientes?page=1&size=25&search=...
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int size = 25, [FromQuery] string? search = null)
        {
            // El repositorio debería buscar ahora por Nombre, Apellido o CUIL en el 'search'
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
            // Cambio de lógica: Ahora validamos por CUIL
            if (!string.IsNullOrEmpty(dto.CUIL) && await _clienteRepo.ExisteCuilAsync(dto.CUIL))
            {
                return BadRequest("El CUIL ya se encuentra registrado.");
            }

            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                CUIL = dto.CUIL,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Localidad = dto.Localidad,
                Provincia = dto.Provincia,
                CodigoPostal = dto.CodigoPostal,
                FechaNacimiento = dto.FechaNacimiento,
                CondicionIVA = dto.CondicionIVA,
                LimiteCredito = dto.LimiteCredito,
                Observaciones = dto.Observaciones,
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

            // Validación de CUIL duplicado al editar (evitar chocar con otros, ignorando el propio)
            if (!string.IsNullOrEmpty(dto.CUIL) && dto.CUIL != clienteExistente.CUIL)
            {
                if (await _clienteRepo.ExisteCuilAsync(dto.CUIL))
                    return BadRequest("El nuevo CUIL ya está siendo usado por otro cliente.");
            }

            clienteExistente.Nombre = dto.Nombre;
            clienteExistente.Apellido = dto.Apellido;
            clienteExistente.DNI = dto.DNI;
            clienteExistente.CUIL = dto.CUIL;
            clienteExistente.Email = dto.Email;
            clienteExistente.Telefono = dto.Telefono;
            clienteExistente.Direccion = dto.Direccion;
            clienteExistente.Localidad = dto.Localidad;
            clienteExistente.Provincia = dto.Provincia;
            clienteExistente.CodigoPostal = dto.CodigoPostal;
            clienteExistente.FechaNacimiento = dto.FechaNacimiento;
            clienteExistente.CondicionIVA = dto.CondicionIVA;
            clienteExistente.LimiteCredito = dto.LimiteCredito;
            clienteExistente.Observaciones = dto.Observaciones;
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