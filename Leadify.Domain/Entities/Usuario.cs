using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace Leadify.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public int PerfilId { get; set; }

        [ForeignKey("PerfilId")]
        public virtual Perfiles? Perfil { get; set; }

        public string? Telefono { get; set; }
        public string? FotoPerfil { get; set; }
        public string? AreaSector { get; set; }
        public string? Observaciones { get; set; }
    }
}
