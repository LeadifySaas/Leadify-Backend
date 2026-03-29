using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class UserRegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; }
    }
}
