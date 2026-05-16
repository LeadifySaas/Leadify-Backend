using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Leadify.Domain.Entities;

namespace Leadify.Infrastructure.Configurations
{
    public class MenuesXPerfilConfiguration : IEntityTypeConfiguration<MenuesXPerfil>
    {
        public void Configure(EntityTypeBuilder<MenuesXPerfil> builder)
        {
            builder.ToTable("MenuesXPerfil"); // Nombre exacto de la DB
            builder.HasKey(x => x.idMenuesXPerfil);

            builder.HasOne(x => x.Perfil)
                   .WithMany(x => x.MenuesXPerfil)
                   .HasForeignKey(x => x.idPerfil);

            builder.HasOne(x => x.Menu)
                   .WithMany(x => x.MenuesXPerfil)
                   .HasForeignKey(x => x.idMenu);
        }
    }
}