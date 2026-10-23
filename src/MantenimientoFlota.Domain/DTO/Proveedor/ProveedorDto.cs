namespace MantenimientoFlota.Domain.DTO.Proveedor;

/// <summary>Contrato de proveedor.</summary>
public sealed class ProveedorDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Persona o medio de contacto del proveedor.</summary>
    /// <example>María Rodríguez</example>
    public string Contacto { get; set; } = string.Empty;

    /// <summary>Dirección del proveedor.</summary>
    /// <example>San José</example>
    public string Direccion { get; set; } = string.Empty;

    /// <summary>Condiciones comerciales del proveedor.</summary>
    /// <example>Contado</example>
    public string CondicionesGenerales { get; set; } = string.Empty;
}
