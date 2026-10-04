namespace MantenimientoFlota.Application.DTOs.Repuesto;

public sealed class RepuestoDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public decimal StockActual { get; set; }

    public decimal StockMinimo { get; set; }
}
