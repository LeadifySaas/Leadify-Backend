using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Application.DTOs
{
    public class MenuNodeDto
    {
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string? Ruta { get; set; }
        public string? Icono { get; set; }
        public List<SubMenuDto> SubMenues { get; set; } = new();
    }

    public class SubMenuDto
    {
        public string Nombre { get; set; }
        public string? Ruta { get; set; }
    }
}
