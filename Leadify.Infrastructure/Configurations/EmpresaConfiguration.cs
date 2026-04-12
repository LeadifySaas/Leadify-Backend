using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leadify.Infrastructure.Data.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("Empresas");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RazonSocial)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(e => e.Cuit)
                .IsRequired()
                .HasMaxLength(20);

            // Filtro global para ignorar los registros con Activo = false
            builder.HasQueryFilter(e => e.Activo);

            // Relación con Cliente (Muchos a Uno)
            builder.HasOne(e => e.Cliente)
                .WithMany() // Si en la clase Cliente agregas ICollection<Empresa>, acá va .WithMany(c => c.Empresas)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict); // Evita el borrado en cascada accidental
        }
    }
}