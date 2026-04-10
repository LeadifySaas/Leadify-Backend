using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class RemitoItemsDto
    {
        public int ArticuloId { get; set; }
        public decimal Cantidad { get; set; }
        public string? Notas { get; set; } 
    }

    public class RemitoItemResponseDto
    {
        public string ArticuloNombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public string? Notas { get; set; }
    }
}
