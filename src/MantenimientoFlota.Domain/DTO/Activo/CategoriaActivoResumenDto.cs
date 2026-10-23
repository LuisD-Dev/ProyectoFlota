namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de categoria activo resumen.</summary>
public sealed class CategoriaActivoResumenDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Intervalo de mantenimiento de la categoría, expresado en kilómetros.</summary>
    /// <example>5000</example>
    public int IntervaloMantenimientoKm { get; set; }
}
