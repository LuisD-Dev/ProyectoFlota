namespace MantenimientoFlota.Domain.Entities;

public class Repuesto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string UnidadMedida { get; set; } = string.Empty;

    public decimal StockActual { get; set; }

    public decimal StockMinimo { get; set; }

    public ICollection<RepuestoProveedor> RepuestoProveedores { get; set; } =
        new List<RepuestoProveedor>();
}
