namespace MantenimientoFlota.Application.DTOs.Activo;

public sealed class CategoriaActivoResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int IntervaloMantenimientoKm { get; set; }
}
