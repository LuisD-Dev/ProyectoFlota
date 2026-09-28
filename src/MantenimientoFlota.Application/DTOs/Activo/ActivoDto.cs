using MantenimientoFlota.Domain.Enums;

namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class ActivoDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateOnly FechaAdquisicion { get; set; }

    public decimal ValorReferencia { get; set; }

    public EstadoActivo Estado { get; set; }

    public string ImagenUrl { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public int KilometrajeActual { get; set; }

    public int CategoriaActivoId { get; set; }

    public string CategoriaActivoNombre { get; set; } = string.Empty;
}
