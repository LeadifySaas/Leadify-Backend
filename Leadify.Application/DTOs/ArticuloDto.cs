using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class ArticuloDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string UnidadMedida { get; set; } = "Unidades";

        public decimal PrecioVenta { get; set; }
        public decimal StockActual { get; set; }
        public bool Activo { get; set; }
    }
}
