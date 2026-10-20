namespace MantenimientoFlota.Application.DTOs.Repuesto;

public sealed class RepuestoDetalleDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal StockActual { get; set; }
    public decimal StockMinimo { get; set; }
    public bool StockBajo { get; set; }
    public decimal FaltanteStockMinimo { get; set; }
    public int CantidadProveedores { get; set; }
    public IReadOnlyList<ProveedorRepuestoResumenDto> Proveedores { get; set; } =
        new List<ProveedorRepuestoResumenDto>();
}
