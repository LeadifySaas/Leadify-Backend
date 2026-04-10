using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class RemitoItem
    {
        public int Id { get; set; }

        public int RemitoId { get; set; }
        public Remito Remito { get; set; } = null!;

        public int ArticuloId { get; set; }
        public Articulo Articulo { get; set; } = null!;

        public decimal Cantidad { get; set; }

        public string? Notas { get; set; } 
    }
}
