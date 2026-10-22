using MantenimientoFlota.Domain.Constants;
using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Infrastructure.Data.Seed;

public static class CatalogSeeder
{
    public static void SeedCatalogs(this ModelBuilder modelBuilder)
    {
        // Id técnico no cero para HasData; Codigo preserva el valor público y persistido.
        modelBuilder.Entity<EstadoActivoCatalogo>().HasData(
            new EstadoActivoCatalogo { Id = 1, Codigo = EstadoActivo.Operativo, Nombre = "Operativo" },
            new EstadoActivoCatalogo { Id = 2, Codigo = EstadoActivo.EnMantenimiento, Nombre = "En Mantenimiento" },
            new EstadoActivoCatalogo { Id = 3, Codigo = EstadoActivo.FueraDeServicio, Nombre = "Fuera de Servicio" },
            new EstadoActivoCatalogo { Id = 4, Codigo = EstadoActivo.DadoDeBaja, Nombre = "Dado de Baja" });
        modelBuilder.Entity<EstadoOrdenCatalogo>().HasData(
            new EstadoOrdenCatalogo { Id = 1, Codigo = EstadoOrdenMantenimiento.Solicitada, Nombre = "Solicitada" },
            new EstadoOrdenCatalogo { Id = 2, Codigo = EstadoOrdenMantenimiento.Diagnosticada, Nombre = "Diagnosticada" },
            new EstadoOrdenCatalogo { Id = 3, Codigo = EstadoOrdenMantenimiento.Aprobada, Nombre = "Aprobada" },
            new EstadoOrdenCatalogo { Id = 4, Codigo = EstadoOrdenMantenimiento.Rechazada, Nombre = "Rechazada" },
            new EstadoOrdenCatalogo { Id = 5, Codigo = EstadoOrdenMantenimiento.EnEjecucion, Nombre = "En Ejecución" },
            new EstadoOrdenCatalogo { Id = 6, Codigo = EstadoOrdenMantenimiento.EnEsperaDeRepuesto, Nombre = "En Espera de Repuesto" },
            new EstadoOrdenCatalogo { Id = 7, Codigo = EstadoOrdenMantenimiento.Completada, Nombre = "Completada" },
            new EstadoOrdenCatalogo { Id = 8, Codigo = EstadoOrdenMantenimiento.Cancelada, Nombre = "Cancelada" });
        modelBuilder.Entity<Rol>().HasData(
            new Rol { Id = SeedIds.RolAdministrador, Nombre = "Administrador" },
            new Rol { Id = SeedIds.RolCoordinadorTecnico, Nombre = "Coordinador/Técnico" },
            new Rol { Id = SeedIds.RolSolicitanteConsulta, Nombre = "Solicitante/Consulta" });
    }
}
