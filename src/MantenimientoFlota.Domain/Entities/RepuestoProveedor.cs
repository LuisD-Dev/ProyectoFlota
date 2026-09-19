namespace MantenimientoFlota.Domain.Entities;

public class RepuestoProveedor
{
    public int RepuestoId { get; set; }

    public int ProveedorId { get; set; }

    public decimal PrecioOfrecido { get; set; }

    public int TiempoEntregaDias { get; set; }

    public Repuesto Repuesto { get; set; } = null!;

    public Proveedor Proveedor { get; set; } = null!;
}
