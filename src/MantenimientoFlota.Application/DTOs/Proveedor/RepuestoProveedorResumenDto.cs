namespace MantenimientoFlota.Application.DTOs.Proveedor;

public sealed class RepuestoProveedorResumenDto
{
    public int RepuestoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioOfrecido { get; set; }
    public int TiempoEntregaDias { get; set; }
}
