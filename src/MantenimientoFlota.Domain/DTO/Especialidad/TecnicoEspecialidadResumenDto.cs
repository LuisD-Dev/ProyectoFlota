namespace MantenimientoFlota.Domain.DTO.Especialidad;

/// <summary>Contrato de tecnico especialidad resumen.</summary>
public sealed class TecnicoEspecialidadResumenDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int TecnicoId { get; set; }
    /// <summary>Nombre completo del técnico.</summary>
    /// <example>María Rodríguez</example>
    public string NombreCompleto { get; set; } = string.Empty;
    /// <summary>Nivel de certificación del técnico en la especialidad.</summary>
    /// <example>Avanzado</example>
    public string NivelCertificacion { get; set; } = string.Empty;
    /// <summary>Indica si el técnico está disponible.</summary>
    /// <example>true</example>
    public bool Disponible { get; set; }
}
