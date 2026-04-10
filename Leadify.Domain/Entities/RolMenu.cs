using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class RolMenu
    {
        public int Id { get; set; }
        public int RolId { get; set; }
        public int MenuId { get; set; }
        public bool PuedeLeer { get; set; }
        public bool PuedeEscribir { get; set; }
        public bool PuedeEliminar { get; set; }

        public Rol? Rol { get; set; }
        public Menu? Menu { get; set; }
    }
}
