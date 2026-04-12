using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; }

        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }
        public string? AreaSector { get; set; }

        public string? Observaciones { get; set; }
    }
}
