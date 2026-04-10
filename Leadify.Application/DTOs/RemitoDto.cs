using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class RemitoDto
    {
        public string NumeroRemito { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public int SedeId { get; set; }
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string? Observaciones { get; set; }
        public List<RemitoItemsDto> Items { get; set; } = new();
        public bool Activo { get; set; }

    }

    public class RemitoResponseDto
    {
        public int Id { get; set; }
        public string NumeroRemito { get; set; } = string.Empty;
        public string ClienteNombre { get; set; } = string.Empty;
        public string SedeNombre { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public List<RemitoItemResponseDto> Items { get; set; } = new();
    }
}
