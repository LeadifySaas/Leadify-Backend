using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class ProveedorArchivo
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string UrlRelativa { get; set; } = string.Empty; // Ej: "proveedores/44/foto.jpg"
        public string Extension { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }
}
