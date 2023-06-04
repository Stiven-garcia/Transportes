using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Transportes.Models;

public partial class TransportesContext : DbContext
{
    public TransportesContext()
    {
    }

    public TransportesContext(DbContextOptions<TransportesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Destino> Destinos { get; set; }

    public virtual DbSet<Envio> Envios { get; set; }

    public virtual DbSet<TipoDestino> TipoDestinos { get; set; }

    public virtual DbSet<TipoProducto> TipoProductos { get; set; }

    public virtual DbSet<TipoVehiculo> TipoVehiculos { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-LF4N6N3I;Initial Catalog=Transportes;Integrated Security=True;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Cedula);

            entity.ToTable("Cliente");

            entity.Property(e => e.Cedula).ValueGeneratedNever();
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombres)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Destino>(entity =>
        {
            entity.HasKey(e => e.IdDestino);

            entity.ToTable("Destino");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.TipoDestinoNavigation).WithMany(p => p.Destinos)
                .HasForeignKey(d => d.TipoDestino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Destino_TipoDestino");
        });

        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.NumeroGuia);

            entity.ToTable("Envio");

            entity.Property(e => e.NumeroGuia)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FechaEntrega).HasColumnType("date");
            entity.Property(e => e.FechaRegistro).HasColumnType("date");
            entity.Property(e => e.Vehiculo)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.CedulaClienteNavigation).WithMany(p => p.Envios)
                .HasForeignKey(d => d.CedulaCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_Cliente");

            entity.HasOne(d => d.DestinoNavigation).WithMany(p => p.Envios)
                .HasForeignKey(d => d.Destino)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_Destino");

            entity.HasOne(d => d.TipoProductoNavigation).WithMany(p => p.Envios)
                .HasForeignKey(d => d.TipoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_TipoProducto");

            entity.HasOne(d => d.VehiculoNavigation).WithMany(p => p.Envios)
                .HasForeignKey(d => d.Vehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Envio_Vehiculo");
        });

        modelBuilder.Entity<TipoDestino>(entity =>
        {
            entity.HasKey(e => e.IdTipoDestino);

            entity.ToTable("TipoDestino");

            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoProducto>(entity =>
        {
            entity.HasKey(e => e.IdTipoProducto);

            entity.ToTable("TipoProducto");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoVehiculo>(entity =>
        {
            entity.HasKey(e => e.IdTipoVehiculo);

            entity.ToTable("TipoVehiculo");

            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Placa);

            entity.ToTable("Vehiculo");

            entity.Property(e => e.Placa)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.TipoVehiculoNavigation).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.TipoVehiculo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehiculo_TipoVehiculo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
