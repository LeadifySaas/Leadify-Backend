using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class MenuPermisoDto
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int? IdPadre { get; set; }
        public int Orden { get; set; } 

        public bool Ver { get; set; }
        public bool Crear { get; set; }
        public bool Editar { get; set; }
        public bool Eliminar { get; set; }

        public List<MenuPermisoDto> SubPermisos { get; set; } = new List<MenuPermisoDto>();
    }

}
