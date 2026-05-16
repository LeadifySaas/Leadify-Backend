using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public string? Descripcion { get; set; }

        //public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        //public ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();
    }
}
