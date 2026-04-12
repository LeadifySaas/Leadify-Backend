using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leadify.Domain.Entities
{
    public class Contacto
    {
        [Key]
        public int Id { get; set; }

        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        public int? EmpresaId { get; set; }
        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        public string? Puesto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}