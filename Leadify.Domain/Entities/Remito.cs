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
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public string? Observaciones { get; set; }

        // 🔐 Auditoría
        public int CreadoPorUsuarioId { get; set; }
        public Usuario CreadoPorUsuario { get; set; } = null!;
        //// 🚚 Logística
        //public string? Transporte { get; set; }
        //public string? Patente { get; set; }
        //public string? Chofer { get; set; }

        //// 👤 Recepción
        //public string? RecibidoPor { get; set; }
        //public string? DocumentoReceptor { get; set; }

        //// 🔗 Relaciones
        //public int? PedidoId { get; set; }
        public List<RemitoItem> Items { get; set; } = new();
    }
}
