namespace MantenimientoFlota.Domain.DTO.RepuestoProveedor;

public sealed class RepuestoProveedorDto
{
    public int RepuestoId { get; set; }

    public string RepuestoNombre { get; set; } = string.Empty;

    public int ProveedorId { get; set; }

    public string ProveedorNombre { get; set; } = string.Empty;

    public decimal PrecioOfrecido { get; set; }

    public int TiempoEntregaDias { get; set; }
}
