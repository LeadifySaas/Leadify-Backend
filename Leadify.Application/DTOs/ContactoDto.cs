using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class ContactoDto
    {
        public int Id { get; set; }
        public int? ClienteId { get; set; }
        public int? EmpresaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Puesto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
        public bool Activo { get; set; }
    }
}
