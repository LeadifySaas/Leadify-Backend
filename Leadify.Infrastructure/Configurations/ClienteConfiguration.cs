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

            builder.Property(c => c.RazonSocial).IsRequired().HasMaxLength(150);
            builder.Property(c => c.CUIT).IsRequired().HasMaxLength(20);
            builder.HasIndex(c => c.CUIT).IsUnique(); 

            builder.Property(c => c.Email).HasMaxLength(100);
            builder.Property(c => c.Telefono).HasMaxLength(50);
        }
    }
}