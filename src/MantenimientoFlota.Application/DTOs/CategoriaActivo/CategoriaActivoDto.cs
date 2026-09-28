namespace MantenimientoFlota.Application.DTOs.CategoriaActivo;

public sealed class CategoriaActivoDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public int IntervaloMantenimientoKm { get; set; }
}
