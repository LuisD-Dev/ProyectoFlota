namespace MantenimientoFlota.Domain.DTO.Especialidad;

/// <summary>Contrato de especialidad.</summary>
public sealed class EspecialidadDto
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
}
