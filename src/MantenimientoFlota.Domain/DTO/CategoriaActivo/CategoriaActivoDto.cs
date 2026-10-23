namespace MantenimientoFlota.Domain.DTO.CategoriaActivo;

/// <summary>Contrato de categoria activo.</summary>
public sealed class CategoriaActivoDto
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

    /// <summary>Intervalo de mantenimiento de la categoría, expresado en kilómetros.</summary>
    /// <example>5000</example>
    public int IntervaloMantenimientoKm { get; set; }
}
