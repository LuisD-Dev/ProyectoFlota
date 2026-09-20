using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class EspecialidadConfiguration : IEntityTypeConfiguration<Especialidad>
{
    public void Configure(EntityTypeBuilder<Especialidad> builder)
    {
        builder.ToTable("Especialidades");

        builder.HasKey(especialidad => especialidad.Id);

        builder.Property(especialidad => especialidad.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(especialidad => especialidad.Descripcion)
            .HasMaxLength(500);
    }
}
