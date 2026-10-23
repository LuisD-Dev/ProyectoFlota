namespace MantenimientoFlota.Domain.DTO.Especialidad;

/// <summary>Contrato de especialidad detalle.</summary>
public sealed class EspecialidadDetalleDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Descripción del recurso.</summary>
    /// <example>Unidad de mantenimiento</example>
    public string Descripcion { get; set; } = string.Empty;
    /// <summary>Técnicos asociados a la especialidad.</summary>
    public IReadOnlyList<TecnicoEspecialidadResumenDto> Tecnicos { get; set; } =
        new List<TecnicoEspecialidadResumenDto>();
}
