using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Persistence;

public class MantenimientoFlotaDbContext : DbContext
{
    public MantenimientoFlotaDbContext(
        DbContextOptions<MantenimientoFlotaDbContext> options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MantenimientoFlotaDbContext).Assembly);
    }
}
