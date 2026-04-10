using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Link { get; set; }
        public bool IsVisible { get; set; }
        public bool IsTitle { get; set; }
        public bool Expanded { get; set; }
        public string? Badge { get; set; }
        public int? ParentId { get; set; }
        public int? MenuId { get; set; }
        public int RolId { get; set; }

        public ICollection<RolMenu> RolMenus { get; set; } = new List<RolMenu>();
    }
}
