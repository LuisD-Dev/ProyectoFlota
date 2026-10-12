using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MantenimientoFlota.Infrastructure.Persistence.Seed;

public static class DevelopmentSeeder
{
    public static void Seed(MantenimientoFlotaDbContext context)
    {
        SeedAsync(context, CancellationToken.None).GetAwaiter().GetResult();
    }

    public static async Task SeedAsync(
        MantenimientoFlotaDbContext context,
        CancellationToken cancellationToken = default)
    {
        IDbContextTransaction? ownedTransaction = null;

        if (context.Database.CurrentTransaction is null)
        {
            ownedTransaction = await context.Database.BeginTransactionAsync(
                cancellationToken);
        }

        try
        {
            var alreadySeeded = await context.OrdenesMantenimiento
                .AsNoTracking()
                .AnyAsync(
                    orden => orden.Codigo == DemoData.SeedMarkerOrderCode,
                    cancellationToken);

            if (alreadySeeded)
            {
                if (ownedTransaction is not null)
                {
                    await ownedTransaction.CommitAsync(cancellationToken);
                }

                return;
            }

            var categorias = DemoData.CreateCategorias();
            context.CategoriasActivo.AddRange(categorias);
            await context.SaveChangesAsync(cancellationToken);

            var especialidades = DemoData.CreateEspecialidades();
            context.Especialidades.AddRange(especialidades);
            await context.SaveChangesAsync(cancellationToken);

            var tecnicos = DemoData.CreateTecnicos();
            context.Tecnicos.AddRange(tecnicos);
            await context.SaveChangesAsync(cancellationToken);

            var repuestos = DemoData.CreateRepuestos();
            context.Repuestos.AddRange(repuestos);
            await context.SaveChangesAsync(cancellationToken);

            var proveedores = DemoData.CreateProveedores();
            context.Proveedores.AddRange(proveedores);
            await context.SaveChangesAsync(cancellationToken);

            var categoriaIds = categorias.ToDictionary(
                categoria => categoria.Nombre,
                categoria => categoria.Id);
            var activos = DemoData.CreateActivos(categoriaIds);
            context.Activos.AddRange(activos);
            await context.SaveChangesAsync(cancellationToken);

            var tecnicoIds = tecnicos.ToDictionary(
                tecnico => tecnico.Identificacion,
                tecnico => tecnico.Id);
            var especialidadIds = especialidades.ToDictionary(
                especialidad => especialidad.Nombre,
                especialidad => especialidad.Id);
            var tecnicoEspecialidades = DemoData.CreateTecnicoEspecialidades(
                tecnicoIds,
                especialidadIds);
            context.TecnicoEspecialidades.AddRange(tecnicoEspecialidades);
            await context.SaveChangesAsync(cancellationToken);

            var repuestoIds = repuestos.ToDictionary(
                repuesto => repuesto.Codigo,
                repuesto => repuesto.Id);
            var proveedorIds = proveedores.ToDictionary(
                proveedor => proveedor.Nombre,
                proveedor => proveedor.Id);
            var repuestoProveedores = DemoData.CreateRepuestoProveedores(
                repuestoIds,
                proveedorIds);
            context.RepuestoProveedores.AddRange(repuestoProveedores);
            await context.SaveChangesAsync(cancellationToken);

            var activoIds = activos.ToDictionary(
                activo => activo.Codigo,
                activo => activo.Id);
            var ordenes = DemoData.CreateOrdenesMantenimiento(activoIds);
            context.OrdenesMantenimiento.AddRange(ordenes);
            await context.SaveChangesAsync(cancellationToken);

            if (ownedTransaction is not null)
            {
                await ownedTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            if (ownedTransaction is not null)
            {
                await ownedTransaction.RollbackAsync(CancellationToken.None);
            }

            throw;
        }
        finally
        {
            if (ownedTransaction is not null)
            {
                await ownedTransaction.DisposeAsync();
            }
        }
    }
}
