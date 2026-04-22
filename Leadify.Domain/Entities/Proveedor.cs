using Leadify.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Proveedor : AuditableEntity 
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string Cuit { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Rubro { get; set; }
        public bool Activo { get; set; } = true;

        public List<ProveedorArchivo> Archivos { get; set; } = new();
    }
}
