using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class RepuestoProveedorConfiguration
    : IEntityTypeConfiguration<RepuestoProveedor>
{
    public void Configure(EntityTypeBuilder<RepuestoProveedor> builder)
    {
        builder.ToTable("RepuestoProveedores", table =>
        {
            table.HasCheckConstraint(
                "CK_RepuestoProveedores_PrecioOfrecido_NoNegativo",
                "[PrecioOfrecido] >= 0");

            table.HasCheckConstraint(
                "CK_RepuestoProveedores_TiempoEntregaDias_NoNegativo",
                "[TiempoEntregaDias] >= 0");
        });

        builder.HasKey(repuestoProveedor => new
        {
            repuestoProveedor.RepuestoId,
            repuestoProveedor.ProveedorId
        });

        builder.Property(repuestoProveedor => repuestoProveedor.PrecioOfrecido)
            .HasPrecision(18, 2);

        builder.Property(repuestoProveedor => repuestoProveedor.TiempoEntregaDias)
            .IsRequired();

        builder.HasOne(repuestoProveedor => repuestoProveedor.Repuesto)
            .WithMany(repuesto => repuesto.RepuestoProveedores)
            .HasForeignKey(repuestoProveedor => repuestoProveedor.RepuestoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(repuestoProveedor => repuestoProveedor.Proveedor)
            .WithMany(proveedor => proveedor.RepuestoProveedores)
            .HasForeignKey(repuestoProveedor => repuestoProveedor.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
