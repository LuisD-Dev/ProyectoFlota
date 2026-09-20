using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class TecnicoEspecialidadConfiguration
    : IEntityTypeConfiguration<TecnicoEspecialidad>
{
    public void Configure(EntityTypeBuilder<TecnicoEspecialidad> builder)
    {
        builder.ToTable("TecnicoEspecialidades");

        builder.HasKey(tecnicoEspecialidad => new
        {
            tecnicoEspecialidad.TecnicoId,
            tecnicoEspecialidad.EspecialidadId
        });

        builder.Property(tecnicoEspecialidad => tecnicoEspecialidad.NivelCertificacion)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(tecnicoEspecialidad => tecnicoEspecialidad.FechaObtencion)
            .HasColumnType("date");

        builder.HasOne(tecnicoEspecialidad => tecnicoEspecialidad.Tecnico)
            .WithMany(tecnico => tecnico.TecnicoEspecialidades)
            .HasForeignKey(tecnicoEspecialidad => tecnicoEspecialidad.TecnicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(tecnicoEspecialidad => tecnicoEspecialidad.Especialidad)
            .WithMany(especialidad => especialidad.TecnicoEspecialidades)
            .HasForeignKey(tecnicoEspecialidad => tecnicoEspecialidad.EspecialidadId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
