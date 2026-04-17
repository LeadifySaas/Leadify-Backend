using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leadify.Domain.Entities
{
    public class Empresa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }

        // Propiedad de navegación: permite acceder a los datos del cliente desde la empresa
        [ForeignKey("ClienteId")]
        public virtual Cliente? Cliente { get; set; }

        [Required]
        [StringLength(250)]
        public string RazonSocial { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Cuit { get; set; } = string.Empty;

        public string? DireccionFiscal { get; set; }

        public string? EmailFacturacion { get; set; }

        public string? CondicionIva { get; set; }

        // El borrado lógico que venimos usando
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}