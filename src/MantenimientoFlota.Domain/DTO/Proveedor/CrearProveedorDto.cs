using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.Proveedor;

/// <summary>Contrato de crear proveedor.</summary>
public sealed class CrearProveedorDto
{
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Persona o medio de contacto del proveedor.</summary>
    /// <example>María Rodríguez</example>
    [MaxLength(200, ErrorMessage = "El contacto no puede superar los 200 caracteres.")]
    public string? Contacto { get; set; }

    /// <summary>Dirección del proveedor.</summary>
    /// <example>San José</example>
    [MaxLength(500, ErrorMessage = "La dirección no puede superar los 500 caracteres.")]
    public string? Direccion { get; set; }

    /// <summary>Condiciones comerciales del proveedor.</summary>
    /// <example>Contado</example>
    [MaxLength(
        1000,
        ErrorMessage = "Las condiciones generales no pueden superar los 1000 caracteres.")]
    public string? CondicionesGenerales { get; set; }
}
