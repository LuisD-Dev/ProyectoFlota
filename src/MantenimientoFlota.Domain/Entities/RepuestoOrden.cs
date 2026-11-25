namespace MantenimientoFlota.Domain.Entities;

public sealed class RepuestoOrden
{
    public int Id { get; set; }
    public int OrdenMantenimientoId { get; set; }
    public OrdenMantenimiento OrdenMantenimiento { get; set; } = null!;
    public int RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public bool Pendiente { get; set; }
}
