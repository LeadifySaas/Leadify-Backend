using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; } 
        public string RazonSocial { get; set; } = string.Empty;
        public string CUIT { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string CondicionIVA { get; set; } = string.Empty; 
        public bool Activo { get; set; }
    }
}
