using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainBitacoraVehicular.Entities;

namespace InfraBitacoraVehicular.Persistence
{
    public class BitacoraVehicularDbContext : DbContext
    {
        public BitacoraVehicularDbContext(DbContextOptions<BitacoraVehicularDbContext> options)
            : base(options)
        {
        }

        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<ContextoEstado> ContextosEstado => Set<ContextoEstado>();
        public DbSet<Estado> Estados => Set<Estado>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Marca> Marcas => Set<Marca>();
        public DbSet<Modelo> Modelos => Set<Modelo>();
        public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
        public DbSet<AsignacionVehiculo> AsignacionesVehiculo => Set<AsignacionVehiculo>();
        public DbSet<Viaje> Viajes => Set<Viaje>();
        public DbSet<TipoCombustible> TiposCombustible => Set<TipoCombustible>();
        public DbSet<CargaCombustible> CargasCombustible => Set<CargaCombustible>();
        public DbSet<TipoMantencion> TiposMantencion => Set<TipoMantencion>();
        public DbSet<Mantencion> Mantenciones => Set<Mantencion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.RolId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255);

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });

            modelBuilder.Entity<ContextoEstado>(entity =>
            {
                entity.ToTable("ContextosEstado");
                entity.HasKey(e => e.ContextoEstadoId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255);

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });

            modelBuilder.Entity<Estado>(entity =>
            {
                entity.ToTable("Estados");
                entity.HasKey(e => e.EstadoId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255);

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.ContextoEstado)
                    .WithMany(c => c.Estados)
                    .HasForeignKey(e => e.ContextoEstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.UsuarioId);

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Correo)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasIndex(e => e.Correo)
                    .IsUnique();

                entity.HasIndex(e => e.NombreUsuario)
                    .IsUnique();

                entity.HasOne(e => e.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.RolId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Estado)
                    .WithMany(e => e.Usuarios)
                    .HasForeignKey(e => e.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Marca>(entity =>
            {
                entity.ToTable("Marcas");
                entity.HasKey(e => e.MarcaId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });

            modelBuilder.Entity<Modelo>(entity =>
            {
                entity.ToTable("Modelos");
                entity.HasKey(e => e.ModeloId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.Marca)
                    .WithMany(m => m.Modelos)
                    .HasForeignKey(e => e.MarcaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.ToTable("Vehiculos");
                entity.HasKey(e => e.VehiculoId);

                entity.Property(e => e.Patente)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Color)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Observacion)
                    .HasMaxLength(255);

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.KilometrajeActual)
                    .IsRequired();

                entity.Property(e => e.Anio)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasIndex(e => e.Patente)
                    .IsUnique();

                entity.HasOne(e => e.Modelo)
                    .WithMany(m => m.Vehiculos)
                    .HasForeignKey(e => e.ModeloId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Estado)
                    .WithMany(e => e.Vehiculos)
                    .HasForeignKey(e => e.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AsignacionVehiculo>(entity =>
            {
                entity.ToTable("AsignacionesVehiculo");
                entity.HasKey(e => e.AsignacionVehiculoId);

                entity.Property(e => e.FechaAsignacion)
                    .IsRequired();

                entity.Property(e => e.Observacion)
                    .HasMaxLength(255);

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.AsignacionesVehiculo)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Vehiculo)
                    .WithMany(v => v.AsignacionesVehiculo)
                    .HasForeignKey(e => e.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Estado)
                    .WithMany(e => e.AsignacionesVehiculo)
                    .HasForeignKey(e => e.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Viaje>(entity =>
            {
                entity.ToTable("Viajes");
                entity.HasKey(e => e.ViajeId);

                entity.Property(e => e.FechaHoraSalida)
                    .IsRequired();

                entity.Property(e => e.KilometrajeSalida)
                    .IsRequired();

                entity.Property(e => e.ObservacionSalida)
                    .HasMaxLength(255);

                entity.Property(e => e.ObservacionLlegada)
                    .HasMaxLength(255);

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Viajes)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Vehiculo)
                    .WithMany(v => v.Viajes)
                    .HasForeignKey(e => e.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Estado)
                    .WithMany(e => e.Viajes)
                    .HasForeignKey(e => e.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.UsuarioId)
                    .HasDatabaseName("UX_Viajes_Usuario_Abierto")
                    .IsUnique()
                    .HasFilter("[FechaHoraLlegada] IS NULL");

                entity.HasIndex(e => e.VehiculoId)
                    .HasDatabaseName("UX_Viajes_Vehiculo_Abierto")
                    .IsUnique()
                    .HasFilter("[FechaHoraLlegada] IS NULL");
            });

            modelBuilder.Entity<TipoCombustible>(entity =>
            {
                entity.ToTable("TiposCombustible");
                entity.HasKey(e => e.TipoCombustibleId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });

            modelBuilder.Entity<CargaCombustible>(entity =>
            {
                entity.ToTable("CargasCombustible");
                entity.HasKey(e => e.CargaCombustibleId);

                entity.Property(e => e.Litros)
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.MontoCargado)
                    .HasColumnType("decimal(12,2)")
                    .IsRequired();

                entity.Property(e => e.FechaCarga)
                    .IsRequired();

                entity.Property(e => e.NombreArchivoBoleta)
                    .HasMaxLength(255);

                entity.Property(e => e.RutaArchivoBoleta)
                    .HasMaxLength(500);

                entity.Property(e => e.Observacion)
                    .HasMaxLength(255);

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.Viaje)
                    .WithMany(v => v.CargasCombustible)
                    .HasForeignKey(e => e.ViajeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.CargasCombustible)
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Vehiculo)
                    .WithMany(v => v.CargasCombustible)
                    .HasForeignKey(e => e.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TipoCombustible)
                    .WithMany(t => t.CargasCombustible)
                    .HasForeignKey(e => e.TipoCombustibleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TipoMantencion>(entity =>
            {
                entity.ToTable("TiposMantencion");
                entity.HasKey(e => e.TipoMantencionId);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(255);

                entity.Property(e => e.Activo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
            });

            modelBuilder.Entity<Mantencion>(entity =>
            {
                entity.ToTable("Mantenciones");
                entity.HasKey(e => e.MantencionId);

                entity.Property(e => e.Detalle)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.Costo)
                    .HasColumnType("decimal(12,2)");

                entity.Property(e => e.FechaIngreso)
                    .IsRequired();

                entity.Property(e => e.KilometrajeVehiculo)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .IsRequired();

                entity.HasOne(e => e.Vehiculo)
                    .WithMany(v => v.Mantenciones)
                    .HasForeignKey(e => e.VehiculoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TipoMantencion)
                    .WithMany(t => t.Mantenciones)
                    .HasForeignKey(e => e.TipoMantencionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Estado)
                    .WithMany(e => e.Mantenciones)
                    .HasForeignKey(e => e.EstadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
