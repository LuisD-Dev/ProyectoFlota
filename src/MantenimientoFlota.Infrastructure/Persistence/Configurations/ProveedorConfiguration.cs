using MantenimientoFlota.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MantenimientoFlota.Infrastructure.Persistence.Configurations;

public sealed class ProveedorConfiguration : IEntityTypeConfiguration<Proveedor>
{
    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores");

        builder.HasKey(proveedor => proveedor.Id);

        builder.Property(proveedor => proveedor.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(proveedor => proveedor.Contacto)
            .HasMaxLength(200);

        builder.Property(proveedor => proveedor.Direccion)
            .HasMaxLength(500);

        builder.Property(proveedor => proveedor.CondicionesGenerales)
            .HasMaxLength(1000);
    }
}
