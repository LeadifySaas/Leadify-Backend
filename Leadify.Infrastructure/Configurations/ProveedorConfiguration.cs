using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Leadify.Domain.Entities;

namespace Leadify.Infrastructure.Configurations
{
    public class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
    {
        public void Configure(EntityTypeBuilder<Proveedor> builder)
        {
            builder.ToTable("Proveedores");
            builder.HasKey(p => p.Id);

            // Razon Social y CUIT (con su índice único)
            builder.Property(p => p.RazonSocial).IsRequired().HasMaxLength(200);
            builder.Property(p => p.Cuit).IsRequired().HasMaxLength(13);
            builder.HasIndex(p => p.Cuit).IsUnique(); // ¡Blinda el CUIT!

            // Otros campos
            builder.Property(p => p.Email).HasMaxLength(100);
            builder.Property(p => p.Telefono).HasMaxLength(50);
            builder.Property(p => p.Rubro).HasMaxLength(100);

            // Si quisieras configurar las fechas explícitamente (opcional)
            builder.Property(p => p.FechaCreacion).IsRequired();
            builder.Property(p => p.FechaActualizacion).IsRequired(false);
        }
    }
}