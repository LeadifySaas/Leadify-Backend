using Leadify.Domain.Common;
using Leadify.Domain.Entities;
using Leadify.Infrastructure.Configurations;
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
        public DbSet<Articulo> Articulos { get; set; }

        public DbSet<ArticulosInternos> ArticulosInternos { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<RemitoItem> RemitoItems { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<Perfiles> Perfiles { get; set; }
        public DbSet<MenuesXPerfil> MenuesXPerfil { get; set; }
        public DbSet<Menues> Menues { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<ProveedorArchivo> ProveedorArchivos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- SEGURIDAD Y PERFILES ---

            modelBuilder.Entity<Perfiles>(entity =>
            {
                entity.ToTable("Perfiles");
                entity.HasKey(e => e.IdPerfil);
                entity.Property(e => e.IdPerfil).HasColumnName("IdPerfil");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PerfilId).HasColumnName("PerfilId");

                entity.HasOne(u => u.Perfil)
                      .WithMany()
                      .HasForeignKey(u => u.PerfilId);
            });

            // --- MENÚS Y JERARQUÍA ---

            modelBuilder.Entity<Menues>(entity =>
            {
                entity.ToTable("Menues");
                entity.HasKey(e => e.IdMenu);
                entity.Property(e => e.IdMenu).HasColumnName("IdMenu");
                entity.Property(e => e.IdPadre).HasColumnName("IdPadre");

                entity.HasOne(m => m.Padre)
                      .WithMany(m => m.SubMenues)
                      .HasForeignKey(m => m.IdPadre)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // --- RELACIÓN MENÚS X PERFIL (Tabla Intermedia) ---

            modelBuilder.Entity<MenuesXPerfil>(entity =>
            {
                entity.ToTable("MenuesXPerfil");
                entity.HasKey(e => e.idMenuesXPerfil);

                entity.Property(e => e.idMenuesXPerfil).HasColumnName("idMenuesXPerfil");
                entity.Property(e => e.idMenu).HasColumnName("idMenu");
                entity.Property(e => e.idPerfil).HasColumnName("idPerfil");

                
                entity.HasOne(d => d.Menu)
                      .WithMany(m => m.MenuesXPerfil) 
                      .HasForeignKey(d => d.idMenu)
                      .HasConstraintName("FK_MenuesXPerfil_Menues");

                entity.HasOne(d => d.Perfil)
                      .WithMany(p => p.MenuesXPerfil) 
                      .HasForeignKey(d => d.idPerfil)
                      .HasConstraintName("FK_MenuesXPerfil_Perfiles");
            });

            // --- ENTIDADES DE NEGOCIO ---

            modelBuilder.Entity<Empresa>(entity =>
            {
                entity.ToTable("Empresas");
                entity.HasOne(e => e.Cliente)
                      .WithMany()
                      .HasForeignKey(e => e.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Sede>(entity =>
            {
                entity.ToTable("Sedes");
                entity.HasOne(s => s.Cliente)
                      .WithMany(c => c.Sedes)
                      .HasForeignKey(s => s.ClienteId);
            });

            modelBuilder.Entity<RemitoItem>(entity =>
            {
                entity.ToTable("RemitoItems");
                entity.HasOne(ri => ri.Remito)
                      .WithMany(r => r.Items)
                      .HasForeignKey(ri => ri.RemitoId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(ri => ri.Cantidad).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.ToTable("Articulos");
                entity.Property(a => a.StockActual).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Contacto>().ToTable("Contactos");
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }


    }
}
