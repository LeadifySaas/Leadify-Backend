using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NombreRol { get; set; } = string.Empty;
        public bool Activo { get; set; }

        public string? Telefono { get; set; }
        public string? AreaSector { get; set; }
    }
}
