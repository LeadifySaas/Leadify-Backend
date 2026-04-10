using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace Leadify.Infrastructure.Configurations
{
    public class SedeConfiguration : IEntityTypeConfiguration<Sede>
    {
        public void Configure(EntityTypeBuilder<Sede> builder)
        {
            builder.ToTable("Sedes");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Direccion).IsRequired().HasMaxLength(255);

            // Relación 1:N con Cliente
            builder.HasOne(s => s.Cliente)
                   .WithMany(c => c.Sedes)
                   .HasForeignKey(s => s.ClienteId)
                   .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
