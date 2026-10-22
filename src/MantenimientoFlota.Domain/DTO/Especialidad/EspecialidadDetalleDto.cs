namespace MantenimientoFlota.Domain.DTO.Especialidad;

public sealed class EspecialidadDetalleDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public IReadOnlyList<TecnicoEspecialidadResumenDto> Tecnicos { get; set; } =
        new List<TecnicoEspecialidadResumenDto>();
}
