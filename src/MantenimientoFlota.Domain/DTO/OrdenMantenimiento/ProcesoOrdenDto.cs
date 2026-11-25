namespace MantenimientoFlota.Domain.DTO.OrdenMantenimiento;

public sealed class HistorialEstadoDto
{
    public int Id { get; set; }
    public int? EstadoAnterior { get; set; }
    public int EstadoNuevo { get; set; }
    public DateTime FechaUtc { get; set; }
    public int UsuarioId { get; set; }
    public string? Observacion { get; set; }
}

public sealed class RepuestoOrdenDto
{
    public int Id { get; set; }
    public int RepuestoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public bool Pendiente { get; set; }
}

public sealed class TecnicoOrdenDto
{
    public int TecnicoId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public decimal HorasTrabajadas { get; set; }
    public decimal TarifaPorHora { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime FechaAsignacionUtc { get; set; }
}
