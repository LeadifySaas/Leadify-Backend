using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Infrastructure.Configurations
{
    public class RemitoItemConfiguration : IEntityTypeConfiguration<RemitoItem>
    {
        public void Configure(EntityTypeBuilder<RemitoItem> builder)
        {
            builder.ToTable("RemitoItems");
            builder.HasKey(ri => ri.Id);

            builder.Property(ri => ri.Cantidad).HasPrecision(18, 2);
            builder.Property(ri => ri.Notas).HasMaxLength(255);

            // Relación con el Remito Padre
            builder.HasOne(ri => ri.Remito)
                   .WithMany(r => r.Items)
                   .HasForeignKey(ri => ri.RemitoId)
                   .OnDelete(DeleteBehavior.Cascade); 

            // Relación con el Artículo
            builder.HasOne(ri => ri.Articulo)
                   .WithMany(a => a.RemitoItems)
                   .HasForeignKey(ri => ri.ArticuloId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
