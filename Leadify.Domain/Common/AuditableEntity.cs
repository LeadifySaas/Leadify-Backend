using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
    }
}
