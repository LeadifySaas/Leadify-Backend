using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Infrastructure.Configurations
{
    public class RemitoConfiguration : IEntityTypeConfiguration<Remito>
    {
        public void Configure(EntityTypeBuilder<Remito> builder)
        {
            builder.ToTable("Remitos");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.NumeroRemito).IsRequired().HasMaxLength(20);
            builder.HasIndex(r => r.NumeroRemito).IsUnique();

            builder.Property(r => r.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Pendiente");

            // Relaciones con Cliente y Sede
            builder.HasOne(r => r.Cliente)
                   .WithMany(c => c.Remitos)
                   .HasForeignKey(r => r.ClienteId)
                   .OnDelete(DeleteBehavior.Restrict); // No borrar cliente si tiene remitos

            builder.HasOne(r => r.Sede)
                   .WithMany(s => s.Remitos)
                   .HasForeignKey(r => r.SedeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
