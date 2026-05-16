using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Leadify.Domain.Entities
{
    public class MenuesXPerfil
    {
        [Key]
        [Column("idMenuesXPerfil")]
        public int idMenuesXPerfil { get; set; }

        [Column("idMenu")] // Nombre exacto en SQL
        public int idMenu { get; set; }

        [Column("idPerfil")] // Nombre exacto en SQL
        public int idPerfil { get; set; }

        public bool PermisoVer { get; set; }
        public bool PermisoCrear { get; set; }
        public bool PermisoEditar { get; set; }
        public bool PermisoEliminar { get; set; }

        [ForeignKey("idMenu")]
        public virtual Menues? Menu { get; set; }

        [ForeignKey("idPerfil")]
        public virtual Perfiles? Perfil { get; set; }
    }
}
