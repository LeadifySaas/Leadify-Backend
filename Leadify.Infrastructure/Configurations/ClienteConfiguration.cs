using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Leadify.Domain.Entities;

namespace Leadify.Infrastructure.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.Id);

            // Configuramos Nombre y Apellido (Reemplazan a RazonSocial)
            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);

            // Configuramos DNI y CUIL (Reemplazan a CUIT)
            builder.Property(c => c.DNI).HasMaxLength(20);
            builder.Property(c => c.CUIL).HasMaxLength(13);

            // Mantenemos la unicidad pero sobre el CUIL
            builder.HasIndex(c => c.CUIL).IsUnique();

            builder.Property(c => c.Email).HasMaxLength(150);
            builder.Property(c => c.Telefono).HasMaxLength(50);

            // Campos de localización y otros
            builder.Property(c => c.Direccion).HasMaxLength(250);
            builder.Property(c => c.Localidad).HasMaxLength(150);
            builder.Property(c => c.Provincia).HasMaxLength(100);
            builder.Property(c => c.CodigoPostal).HasMaxLength(20);
            builder.Property(c => c.LimiteCredito).HasColumnType("decimal(18,2)");
            builder.Property(c => c.Observaciones).HasMaxLength(2000);
        }
    }
}