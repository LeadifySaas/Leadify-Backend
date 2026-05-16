using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Perfiles
    {
        public int IdPerfil { get; set; } // PK coincidente con tu SELECT
        public string Nombre { get; set; }
        public bool Activo { get; set; }

        public virtual ICollection<MenuesXPerfil> MenuesXPerfil { get; set; } = new List<MenuesXPerfil>();
    }
}
