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

            using var hmac = new HMACSHA512();

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                RolId = dto.RolId,
                Activo = true,
                FechaCreacion = DateTime.Now,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Usuarios.Add(usuario);
            await _context.Set<Usuario>().AddAsync(usuario); // Asegura que use la entidad
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
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsers()
        {
            return await _context.Usuarios.Include(u => u.Rol).ToListAsync();
        }
    }
}