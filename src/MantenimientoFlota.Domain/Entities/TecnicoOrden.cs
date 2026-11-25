namespace MantenimientoFlota.Domain.Entities;

public sealed class TecnicoOrden
{
    public int OrdenMantenimientoId { get; set; }
    public OrdenMantenimiento OrdenMantenimiento { get; set; } = null!;
    public int TecnicoId { get; set; }
    public Tecnico Tecnico { get; set; } = null!;
    public decimal HorasTrabajadas { get; set; }
    public decimal TarifaPorHora { get; set; }
    public DateTime FechaAsignacionUtc { get; set; }
}
