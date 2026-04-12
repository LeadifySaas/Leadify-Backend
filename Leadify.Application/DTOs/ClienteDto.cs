using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? DNI { get; set; }
        public string? CUIL { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Localidad { get; set; }
        public string? Provincia { get; set; }
        public string? CodigoPostal { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? CondicionIVA { get; set; }
        public decimal LimiteCredito { get; set; }
        public string? Observaciones { get; set; }
        public bool Activo { get; set; }
    }
}
