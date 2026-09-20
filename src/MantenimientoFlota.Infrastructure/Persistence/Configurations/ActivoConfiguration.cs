using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class ActivoConfiguration : IEntityTypeConfiguration<Activo>
{
    public void Configure(EntityTypeBuilder<Activo> builder)
    {
        builder.ToTable("Activos");

        builder.HasKey(activo => activo.Id);

        builder.Property(activo => activo.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(activo => activo.Codigo)
            .IsUnique();

        builder.Property(activo => activo.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(activo => activo.Descripcion)
            .HasMaxLength(500);

        builder.Property(activo => activo.FechaAdquisicion)
            .HasColumnType("date");

        builder.Property(activo => activo.ValorReferencia)
            .HasPrecision(18, 2);

        builder.Property(activo => activo.Estado)
            .IsRequired();

        builder.Property(activo => activo.ImagenUrl)
            .HasMaxLength(2048);

        builder.Property(activo => activo.Placa)
            .HasMaxLength(20);

        builder.Property(activo => activo.KilometrajeActual)
            .IsRequired();

        builder.Property(activo => activo.CategoriaActivoId)
            .IsRequired();

        builder.HasOne(activo => activo.CategoriaActivo)
            .WithMany(categoria => categoria.Activos)
            .HasForeignKey(activo => activo.CategoriaActivoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
