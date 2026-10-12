using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class TecnicoConfiguration : IEntityTypeConfiguration<Tecnico>
{
    public void Configure(EntityTypeBuilder<Tecnico> builder)
    {
        builder.ToTable("Tecnicos", table =>
        {
            table.HasCheckConstraint(
                "CK_Tecnicos_TarifaPorHora_NoNegativa",
                "[TarifaPorHora] >= 0");
        });

        builder.HasKey(tecnico => tecnico.Id);

        builder.Property(tecnico => tecnico.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(tecnico => tecnico.Identificacion)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(tecnico => tecnico.Telefono)
            .HasMaxLength(30);

        builder.Property(tecnico => tecnico.Correo)
            .HasMaxLength(254);

        builder.Property(tecnico => tecnico.Disponible)
            .IsRequired();

        builder.Property(tecnico => tecnico.TarifaPorHora)
            .HasPrecision(18, 2);
    }
}
