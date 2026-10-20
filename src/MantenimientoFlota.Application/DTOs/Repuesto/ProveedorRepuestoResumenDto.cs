namespace MantenimientoFlota.Application.DTOs.Repuesto;

public sealed class ProveedorRepuestoResumenDto
{
    public int ProveedorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioOfrecido { get; set; }
    public int TiempoEntregaDias { get; set; }
}
