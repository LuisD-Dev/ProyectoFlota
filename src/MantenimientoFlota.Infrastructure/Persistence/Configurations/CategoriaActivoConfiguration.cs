using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class CategoriaActivoConfiguration : IEntityTypeConfiguration<CategoriaActivo>
{
    public void Configure(EntityTypeBuilder<CategoriaActivo> builder)
    {
        builder.ToTable("CategoriasActivo");

        builder.HasKey(categoria => categoria.Id);

        builder.Property(categoria => categoria.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(categoria => categoria.Nombre)
            .IsUnique();

        builder.Property(categoria => categoria.Descripcion)
            .HasMaxLength(500);

        builder.Property(categoria => categoria.IntervaloMantenimientoKm)
            .IsRequired();
    }
}
