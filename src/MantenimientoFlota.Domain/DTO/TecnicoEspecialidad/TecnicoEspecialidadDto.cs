namespace MantenimientoFlota.Domain.DTO.TecnicoEspecialidad;

/// <summary>Contrato de tecnico especialidad.</summary>
public sealed class TecnicoEspecialidadDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int TecnicoId { get; set; }

    /// <summary>Tecnico nombre completo.</summary>
    public string TecnicoNombreCompleto { get; set; } = string.Empty;

    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int EspecialidadId { get; set; }

    /// <summary>Especialidad nombre.</summary>
    public string EspecialidadNombre { get; set; } = string.Empty;

    /// <summary>Nivel de certificación del técnico en la especialidad.</summary>
    /// <example>Avanzado</example>
    public string NivelCertificacion { get; set; } = string.Empty;

    /// <summary>Fecha de obtención de la certificación.</summary>
    /// <example>2020-01-15</example>
    public DateOnly FechaObtencion { get; set; }
}
