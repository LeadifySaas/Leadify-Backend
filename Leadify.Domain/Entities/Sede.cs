using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Sede
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public string Nombre { get; set; } = string.Empty; 
        public string Direccion { get; set; } = string.Empty;
        public string Localidad { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string ContactoNombre { get; set; } = string.Empty;
        public string ContactoTelefono { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public ICollection<Remito> Remitos { get; set; } = new List<Remito>();

    }
}
