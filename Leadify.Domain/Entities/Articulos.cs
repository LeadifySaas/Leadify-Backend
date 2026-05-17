using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Articulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string UnidadMedida { get; set; } = "Unidades";
        public string? ImagenUrl { get; set; }

        public decimal PrecioVenta { get; set; }
        public decimal StockActual { get; set; }
        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Relación con los items de los remitos
        public virtual ICollection<RemitoItem> RemitoItems { get; set; } = new List<RemitoItem>();
    }
}
