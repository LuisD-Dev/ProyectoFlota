namespace MantenimientoFlota.Domain.DTO.CategoriaActivo;

public sealed class CategoriaActivoDetalleDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int IntervaloMantenimientoKm { get; set; }

    public int CantidadActivos { get; set; }

    public IReadOnlyList<ActivoResumenDto> Activos { get; set; } = new List<ActivoResumenDto>();
}
