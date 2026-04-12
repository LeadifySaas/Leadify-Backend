using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Infrastructure.Configurations
{
    public class ContactoConfiguration : IEntityTypeConfiguration<Contacto>
    {
        public void Configure(EntityTypeBuilder<Contacto> builder)
        {
            builder.ToTable("Contactos");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);

            // Filtro de borrado lógico
            builder.HasQueryFilter(c => c.Activo);

            // Relaciones
            builder.HasOne(c => c.Cliente)
                   .WithMany()
                   .HasForeignKey(c => c.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Empresa)
                   .WithMany()
                   .HasForeignKey(c => c.EmpresaId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
