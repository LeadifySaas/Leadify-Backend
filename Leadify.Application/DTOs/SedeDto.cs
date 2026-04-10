using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class SedeDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Localidad { get; set; }
        public string? Provincia { get; set; }
        public string? CodigoPostal { get; set; }
        public string? ContactoNombre { get; set; }
        public string? ContactoTelefono { get; set; }
        public bool Activo { get; set; }

    }
}
