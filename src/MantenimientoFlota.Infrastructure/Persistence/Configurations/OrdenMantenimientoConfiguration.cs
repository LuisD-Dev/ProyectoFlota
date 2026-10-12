using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class OrdenMantenimientoConfiguration :
    IEntityTypeConfiguration<OrdenMantenimiento>
{
    public void Configure(EntityTypeBuilder<OrdenMantenimiento> builder)
    {
        builder.ToTable("OrdenesMantenimiento", table =>
        {
            table.HasCheckConstraint(
                "CK_OrdenesMantenimiento_CostoTotal_NoNegativo",
                "[CostoTotal] >= 0");
        });

        builder.HasKey(orden => orden.Id);

        builder.Property(orden => orden.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(orden => orden.Codigo)
            .IsUnique();

        builder.Property(orden => orden.ActivoId)
            .IsRequired();

        builder.Property(orden => orden.Motivo)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(orden => orden.TipoMantenimiento)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(orden => orden.FechaSolicitud)
            .HasColumnType("datetime2");

        builder.Property(orden => orden.FechaProgramada)
            .HasColumnType("datetime2");

        builder.Property(orden => orden.FechaCierre)
            .HasColumnType("datetime2");

        builder.Property(orden => orden.Prioridad)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(orden => orden.CostoTotal)
            .HasPrecision(18, 2);

        builder.Property(orden => orden.EstadoActual)
            .IsRequired();

        builder.HasOne(orden => orden.Activo)
            .WithMany(activo => activo.OrdenesMantenimiento)
            .HasForeignKey(orden => orden.ActivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
