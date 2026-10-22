using MantenimientoFlota.Infrastructure.Data.Seed;
using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activo> Activos => Set<Activo>();

    public DbSet<CategoriaActivo> CategoriasActivo => Set<CategoriaActivo>();

    public DbSet<Tecnico> Tecnicos => Set<Tecnico>();

    public DbSet<Especialidad> Especialidades => Set<Especialidad>();

    public DbSet<TecnicoEspecialidad> TecnicoEspecialidades => Set<TecnicoEspecialidad>();

    public DbSet<Repuesto> Repuestos => Set<Repuesto>();

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    public DbSet<RepuestoProveedor> RepuestoProveedores => Set<RepuestoProveedor>();

    public DbSet<OrdenMantenimiento> OrdenesMantenimiento => Set<OrdenMantenimiento>();

    public DbSet<EstadoActivoCatalogo> EstadosActivo => Set<EstadoActivoCatalogo>();
    public DbSet<EstadoOrdenCatalogo> EstadosOrdenMantenimiento => Set<EstadoOrdenCatalogo>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Activo
        modelBuilder.Entity<Activo>(entity =>
        {
            entity.ToTable("Activos");

            entity.HasKey(activo => activo.Id);

            entity.Property(activo => activo.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(activo => activo.Codigo)
                .IsUnique();

            entity.Property(activo => activo.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(activo => activo.Descripcion)
                .HasMaxLength(500);

            entity.Property(activo => activo.FechaAdquisicion)
                .HasColumnType("date");

            entity.Property(activo => activo.ValorReferencia)
                .HasPrecision(18, 2);

            entity.Property(activo => activo.Estado)
                .IsRequired();

            entity.HasOne<EstadoActivoCatalogo>().WithMany()
                .HasForeignKey(activo => activo.Estado)
                .HasPrincipalKey(estado => estado.Codigo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(activo => activo.ImagenUrl)
                .HasMaxLength(2048);

            entity.Property(activo => activo.Placa)
                .HasMaxLength(20);

            entity.Property(activo => activo.KilometrajeActual)
                .IsRequired();

            entity.Property(activo => activo.CategoriaActivoId)
                .IsRequired();

            entity.HasOne(activo => activo.CategoriaActivo)
                .WithMany(categoria => categoria.Activos)
                .HasForeignKey(activo => activo.CategoriaActivoId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // CategoriaActivo
        modelBuilder.Entity<CategoriaActivo>(entity =>
        {
            entity.ToTable("CategoriasActivo", table =>
            {
                table.HasCheckConstraint(
                    "CK_CategoriasActivo_IntervaloMantenimientoKm_NoNegativo",
                    "[IntervaloMantenimientoKm] >= 0");
            });

            entity.HasKey(categoria => categoria.Id);

            entity.Property(categoria => categoria.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(categoria => categoria.Nombre)
                .IsUnique();

            entity.Property(categoria => categoria.Descripcion)
                .HasMaxLength(500);

            entity.Property(categoria => categoria.IntervaloMantenimientoKm)
                .IsRequired();

        });

        // Especialidad
        modelBuilder.Entity<Especialidad>(entity =>
        {
            entity.ToTable("Especialidades");

            entity.HasKey(especialidad => especialidad.Id);

            entity.Property(especialidad => especialidad.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(especialidad => especialidad.Nombre)
                .IsUnique();

            entity.Property(especialidad => especialidad.Descripcion)
                .HasMaxLength(500);

        });

        // EstadoActivoCatalogo
        modelBuilder.Entity<EstadoActivoCatalogo>(entity =>
        {
            entity.ToTable("EstadosActivo");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.HasAlternateKey(x => x.Codigo);
            entity.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Nombre).IsUnique();

        });

        // EstadoOrdenCatalogo
        modelBuilder.Entity<EstadoOrdenCatalogo>(entity =>
        {
            entity.ToTable("EstadosOrdenMantenimiento");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.HasAlternateKey(x => x.Codigo);
            entity.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Nombre).IsUnique();

        });

        // OrdenMantenimiento
        modelBuilder.Entity<OrdenMantenimiento>(entity =>
        {
            entity.ToTable("OrdenesMantenimiento", table =>
            {
                table.HasCheckConstraint(
                    "CK_OrdenesMantenimiento_CostoTotal_NoNegativo",
                    "[CostoTotal] >= 0");
            });

            entity.HasKey(orden => orden.Id);

            entity.Property(orden => orden.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(orden => orden.Codigo)
                .IsUnique();

            entity.Property(orden => orden.ActivoId)
                .IsRequired();

            entity.Property(orden => orden.Motivo)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(orden => orden.TipoMantenimiento)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(orden => orden.FechaSolicitud)
                .HasColumnType("datetime2");

            entity.Property(orden => orden.FechaProgramada)
                .HasColumnType("datetime2");

            entity.Property(orden => orden.FechaCierre)
                .HasColumnType("datetime2");

            entity.Property(orden => orden.Prioridad)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(orden => orden.CostoTotal)
                .HasPrecision(18, 2);

            entity.Property(orden => orden.EstadoActual)
                .IsRequired();

            entity.HasOne<EstadoOrdenCatalogo>().WithMany()
                .HasForeignKey(orden => orden.EstadoActual)
                .HasPrincipalKey(estado => estado.Codigo)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(orden => orden.Activo)
                .WithMany(activo => activo.OrdenesMantenimiento)
                .HasForeignKey(orden => orden.ActivoId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // Proveedor
        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("Proveedores");

            entity.HasKey(proveedor => proveedor.Id);

            entity.Property(proveedor => proveedor.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(proveedor => proveedor.Contacto)
                .HasMaxLength(200);

            entity.Property(proveedor => proveedor.Direccion)
                .HasMaxLength(500);

            entity.Property(proveedor => proveedor.CondicionesGenerales)
                .HasMaxLength(1000);

        });

        // Repuesto
        modelBuilder.Entity<Repuesto>(entity =>
        {
            entity.ToTable("Repuestos", table =>
            {
                table.HasCheckConstraint(
                    "CK_Repuestos_StockActual_NoNegativo",
                    "[StockActual] >= 0");

                table.HasCheckConstraint(
                    "CK_Repuestos_StockMinimo_NoNegativo",
                    "[StockMinimo] >= 0");
            });

            entity.HasKey(repuesto => repuesto.Id);

            entity.Property(repuesto => repuesto.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(repuesto => repuesto.Codigo)
                .IsUnique();

            entity.Property(repuesto => repuesto.Nombre)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(repuesto => repuesto.UnidadMedida)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(repuesto => repuesto.StockActual)
                .HasPrecision(18, 2);

            entity.Property(repuesto => repuesto.StockMinimo)
                .HasPrecision(18, 2);

        });

        // RepuestoProveedor
        modelBuilder.Entity<RepuestoProveedor>(entity =>
        {
            entity.ToTable("RepuestoProveedores", table =>
            {
                table.HasCheckConstraint(
                    "CK_RepuestoProveedores_PrecioOfrecido_NoNegativo",
                    "[PrecioOfrecido] >= 0");

                table.HasCheckConstraint(
                    "CK_RepuestoProveedores_TiempoEntregaDias_NoNegativo",
                    "[TiempoEntregaDias] >= 0");
            });

            entity.HasKey(repuestoProveedor => new
            {
                repuestoProveedor.RepuestoId,
                repuestoProveedor.ProveedorId
            });

            entity.Property(repuestoProveedor => repuestoProveedor.PrecioOfrecido)
                .HasPrecision(18, 2);

            entity.Property(repuestoProveedor => repuestoProveedor.TiempoEntregaDias)
                .IsRequired();

            entity.HasOne(repuestoProveedor => repuestoProveedor.Repuesto)
                .WithMany(repuesto => repuesto.RepuestoProveedores)
                .HasForeignKey(repuestoProveedor => repuestoProveedor.RepuestoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(repuestoProveedor => repuestoProveedor.Proveedor)
                .WithMany(proveedor => proveedor.RepuestoProveedores)
                .HasForeignKey(repuestoProveedor => repuestoProveedor.ProveedorId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // Rol
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Nombre).IsUnique();

        });

        // Tecnico
        modelBuilder.Entity<Tecnico>(entity =>
        {
            entity.ToTable("Tecnicos", table =>
            {
                table.HasCheckConstraint(
                    "CK_Tecnicos_TarifaPorHora_NoNegativa",
                    "[TarifaPorHora] >= 0");
            });

            entity.HasKey(tecnico => tecnico.Id);

            entity.Property(tecnico => tecnico.NombreCompleto)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(tecnico => tecnico.Identificacion)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(tecnico => tecnico.Telefono)
                .HasMaxLength(30);

            entity.Property(tecnico => tecnico.Correo)
                .HasMaxLength(254);

            entity.Property(tecnico => tecnico.Disponible)
                .IsRequired();

            entity.Property(tecnico => tecnico.TarifaPorHora)
                .HasPrecision(18, 2);

        });

        // TecnicoEspecialidad
        modelBuilder.Entity<TecnicoEspecialidad>(entity =>
        {
            entity.ToTable("TecnicoEspecialidades");

            entity.HasKey(tecnicoEspecialidad => new
            {
                tecnicoEspecialidad.TecnicoId,
                tecnicoEspecialidad.EspecialidadId
            });

            entity.Property(tecnicoEspecialidad => tecnicoEspecialidad.NivelCertificacion)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(tecnicoEspecialidad => tecnicoEspecialidad.FechaObtencion)
                .HasColumnType("date");

            entity.HasOne(tecnicoEspecialidad => tecnicoEspecialidad.Tecnico)
                .WithMany(tecnico => tecnico.TecnicoEspecialidades)
                .HasForeignKey(tecnicoEspecialidad => tecnicoEspecialidad.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tecnicoEspecialidad => tecnicoEspecialidad.Especialidad)
                .WithMany(especialidad => especialidad.TecnicoEspecialidades)
                .HasForeignKey(tecnicoEspecialidad => tecnicoEspecialidad.EspecialidadId)
                .OnDelete(DeleteBehavior.Restrict);

        });

        // Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Login).IsRequired().HasMaxLength(50);
            entity.HasIndex(x => x.Login).IsUnique();
            entity.Property(x => x.PasswordHash).IsRequired().HasMaxLength(512);
            entity.Property(x => x.NombreCompleto).IsRequired().HasMaxLength(150);
            entity.Property(x => x.Correo).IsRequired().HasMaxLength(150);
            entity.HasIndex(x => x.Correo).IsUnique();
            entity.Property(x => x.Activo).IsRequired();
            entity.Property(x => x.FechaCreacionUtc).IsRequired();
            entity.HasOne(x => x.Rol).WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.RolId).OnDelete(DeleteBehavior.Restrict);

        });

        modelBuilder.SeedCatalogs();
    }
}
