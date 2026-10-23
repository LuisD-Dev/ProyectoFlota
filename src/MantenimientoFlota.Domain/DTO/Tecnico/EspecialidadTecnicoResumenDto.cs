namespace MantenimientoFlota.Domain.DTO.Tecnico;

/// <summary>Contrato de especialidad tecnico resumen.</summary>
public sealed class EspecialidadTecnicoResumenDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int EspecialidadId { get; set; }
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Nivel de certificación del técnico en la especialidad.</summary>
    /// <example>Avanzado</example>
    public string NivelCertificacion { get; set; } = string.Empty;
    /// <summary>Fecha de obtención de la certificación.</summary>
    /// <example>2020-01-15</example>
    public DateOnly FechaObtencion { get; set; }
    /// <summary>Antigüedad expresada en años completos.</summary>
    public int AntiguedadAnios { get; set; }
}
