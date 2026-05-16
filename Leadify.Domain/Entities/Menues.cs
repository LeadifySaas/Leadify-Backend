using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Menues
    {
        [Key]
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string? Accion { get; set; }
        public string? Controller { get; set; }
        public string? Ruta { get; set; } // Agregado para el frontend
        public int Orden { get; set; }
        public bool Estado { get; set; }
        public bool DropDown { get; set; }
        public string? Icono { get; set; }
        public string? Parametros { get; set; }
        public int? IdPadre { get; set; }

        // Relaciones
        public virtual Menues? Padre { get; set; }
        public virtual ICollection<Menues> SubMenues { get; set; }
        public virtual ICollection<MenuesXPerfil> MenuesXPerfil { get; set; } = new List<MenuesXPerfil>();
    }
}
