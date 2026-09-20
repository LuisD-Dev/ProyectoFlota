using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class RepuestoConfiguration : IEntityTypeConfiguration<Repuesto>
{
    public void Configure(EntityTypeBuilder<Repuesto> builder)
    {
        builder.ToTable("Repuestos", table =>
        {
            table.HasCheckConstraint(
                "CK_Repuestos_StockActual_NoNegativo",
                "[StockActual] >= 0");

            table.HasCheckConstraint(
                "CK_Repuestos_StockMinimo_NoNegativo",
                "[StockMinimo] >= 0");
        });

        builder.HasKey(repuesto => repuesto.Id);

        builder.Property(repuesto => repuesto.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(repuesto => repuesto.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(repuesto => repuesto.UnidadMedida)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(repuesto => repuesto.StockActual)
            .HasPrecision(18, 2);

        builder.Property(repuesto => repuesto.StockMinimo)
            .HasPrecision(18, 2);
    }
}
