using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public bool Activo { get; set; }
        public int RolId { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Rol? Rol { get; set; }
    }
}
