using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Remito
    {
        public int Id { get; set; }
        public string NumeroRemito { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!; 
        public int SedeId { get; set; }
        public Sede Sede { get; set; } = null!;      
        public DateTime FechaEmision { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string? Observaciones { get; set; }
        public List<RemitoItem> Items { get; set; } = new();
    }
}
