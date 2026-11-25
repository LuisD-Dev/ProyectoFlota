namespace MantenimientoFlota.Domain.Entities;

public sealed class HistorialEstado
{
    public int Id { get; set; }
    public int? OrdenMantenimientoId { get; set; }
    public OrdenMantenimiento? OrdenMantenimiento { get; set; }
    public int? ActivoId { get; set; }
    public Activo? Activo { get; set; }
    public int? EstadoAnterior { get; set; }
    public int EstadoNuevo { get; set; }
    public DateTime FechaUtc { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string? Observacion { get; set; }
}
