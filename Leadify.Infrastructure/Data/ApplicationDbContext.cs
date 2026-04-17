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
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<RemitoItem> RemitoItems { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Contacto> Contactos { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuramos nombres de tablas si son diferentes a las propiedades
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Menu>().ToTable("Menu");
            modelBuilder.Entity<RolMenu>().ToTable("RolMenu");


            // --- Configuración de Empresa ---
            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresas");

                // Borrado lógico global para Empresas
               // entity.HasQueryFilter(e => e.Activo);

                // Relación Cliente -> Empresas (1:N)
                entity.HasOne(e => e.Cliente)
                      .WithMany() // O .WithMany(c => c.Empresas) si agregaste la lista en la entidad Cliente
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict); // Evita borrado accidental si hay datos vinculados
            });

            // Aquí vamos a definir relaciones más complejas si fuera necesario

            modelBuilder.Entity<Sede>()
            .HasOne(s => s.Cliente)
            .WithMany(c => c.Sedes)
            .HasForeignKey(s => s.ClienteId);

            // Relación Remito -> Items (1:N) con borrado en cascada
            modelBuilder.Entity<RemitoItem>()
                .HasOne(ri => ri.Remito)
                .WithMany(r => r.Items)
                .HasForeignKey(ri => ri.RemitoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Precisiones para decimales de Cantidad
            modelBuilder.Entity<RemitoItem>()
                .Property(ri => ri.Cantidad)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Articulo>()
                .Property(a => a.StockActual)
                .HasPrecision(18, 2);


        }
    }
}
