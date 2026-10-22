using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

public sealed class ActivoDetalleDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateOnly FechaAdquisicion { get; set; }
    public decimal ValorReferencia { get; set; }
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int Estado { get; set; }
    public string ImagenUrl { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public int KilometrajeActual { get; set; }
    public int CategoriaActivoId { get; set; }
    public string CategoriaActivoNombre { get; set; } = string.Empty;
    public CategoriaActivoResumenDto Categoria { get; set; } = new();
    public IReadOnlyList<OrdenMantenimientoResumenDto> Ordenes { get; set; } =
        new List<OrdenMantenimientoResumenDto>();
    public int CantidadOrdenes { get; set; }
    public int AntiguedadAnios { get; set; }
}
