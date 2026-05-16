using Leadify.Application.DTOs;
using Leadify.Application.Interfaces;
using Leadify.Domain.Entities;
using Leadify.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Leadify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public UsersController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Usuario>> Register(UserRegisterDto dto)
        {

            if (await _context.Usuarios.AnyAsync(x => x.Email == dto.Email))
                return BadRequest("El email ya está registrado");

            if (!await _context.Perfiles.AnyAsync(r => r.IdPerfil == dto.PerfilId))
                return BadRequest("El rol seleccionado no es válido en la base de datos.");

            if (dto.Password.Length < 8 || !dto.Password.Any(char.IsUpper) || !dto.Password.Any(char.IsDigit))
            {
                return BadRequest("La contraseña debe tener al menos 8 caracteres, una mayúscula y un número.");
            }

            using var hmac = new HMACSHA512();

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PerfilId = dto.PerfilId,
                Activo = true,
                FechaCreacion = DateTime.Now,
                Telefono = dto.Telefono,
                AreaSector = dto.AreaSector,
                FotoPerfil = dto.FotoPerfil,
                Observaciones = dto.Observaciones,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario creado con éxito" });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto) // Agregamos [FromBody] y el DTO
        {
            var token = await _authService.LoginAsync(loginDto.Email, loginDto.Password);

            if (token == null)
                return Unauthorized(new { message = "Email o contraseña incorrectos" });

            return Ok(new { token });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers([FromQuery] string search = "")
        {
            var query = _context.Usuarios
                .Include(u => u.Perfil)
                .AsQueryable();

            // 1. Lógica del Buscador
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(search) ||
                    u.Apellido.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            
            var users = await query
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    NombreRol = u.Perfil.Nombre,
                    Activo = u.Activo,
                    Telefono = u.Telefono,
                    AreaSector = u.AreaSector
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Usuario dto)
        {
            if (id != dto.Id) return BadRequest("El ID no coincide");

            var usuarioDb = await _context.Usuarios.FindAsync(id);
            if (usuarioDb == null) return NotFound("Usuario no encontrado");

            // Actualización de campos permitidos según el perfil
            usuarioDb.Nombre = dto.Nombre;
            usuarioDb.Apellido = dto.Apellido;
            usuarioDb.Email = dto.Email; 
            usuarioDb.PerfilId = dto.PerfilId;
            usuarioDb.Activo = dto.Activo;

            // Campos nuevos del protocolo SQL
            usuarioDb.Telefono = dto.Telefono;
            usuarioDb.AreaSector = dto.AreaSector;
            usuarioDb.Observaciones = dto.Observaciones;
            usuarioDb.FotoPerfil = dto.FotoPerfil;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar: " + ex.Message);
            }

            return Ok(new { message = "Perfil actualizado correctamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
           
            if (usuario == null)
            {
                return NotFound();
            }
            usuario.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
           
            var usuario = await _context.Usuarios
                .Include(u => u.Perfil) 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();

            return Ok(usuario);
        }
    }
}