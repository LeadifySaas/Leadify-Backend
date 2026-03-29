using Leadify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leadify.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Definimos los sets de datos (Tablas)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RolMenu> RolMenus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos nombres de tablas si son diferentes a las propiedades
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Menu>().ToTable("Menu");
            modelBuilder.Entity<RolMenu>().ToTable("RolMenu");

            // Aquí podemos definir relaciones más complejas si fuera necesario
        }
    }
}
