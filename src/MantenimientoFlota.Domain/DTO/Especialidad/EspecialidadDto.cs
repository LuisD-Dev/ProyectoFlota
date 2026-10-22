namespace MantenimientoFlota.Domain.DTO.Especialidad;

public sealed class EspecialidadDto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;
}
