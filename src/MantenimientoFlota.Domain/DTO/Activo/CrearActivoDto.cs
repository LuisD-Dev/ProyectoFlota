using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Validations;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de crear activo.</summary>
public sealed class CrearActivoDto
{
    /// <summary>Código identificador del recurso.</summary>
    /// <example>VEH-001</example>
    [Required(ErrorMessage = "El código es obligatorio.")]
    [MaxLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
    [CodigoActivo]
    [Description("Código único: VEH-, segmentos opcionales de letras ASCII mayúsculas separados por guiones y tres dígitos finales (incluido 000). Ejemplos: VEH-001 y VEH-QA-BAJA-001. Se aceptan espacios exteriores y se eliminan al guardar; máximo 50 caracteres en la entrada, incluidos esos espacios. No se convierten minúsculas.")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del recurso.</summary>
    /// <example>Unidad de mantenimiento</example>
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    /// <summary>Fecha de adquisición del activo.</summary>
    /// <example>2022-01-15</example>
    [Required(ErrorMessage = "La fecha de adquisición es obligatoria.")]
    public DateOnly? FechaAdquisicion { get; set; }

    /// <summary>Valor de referencia del activo.</summary>
    /// <example>1000000</example>
    [Required(ErrorMessage = "El valor de referencia es obligatorio.")]
    [Range(
        typeof(decimal),
        "0",
        "9999999999999999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "El valor de referencia debe ser mayor o igual que cero.")]
    public decimal? ValorReferencia { get; set; }

    /// <summary>Estado numérico del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio y 3 DadoDeBaja.</summary>
    /// <example>0</example>
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [Range(EstadoActivo.Operativo, EstadoActivo.DadoDeBaja, ErrorMessage = "El estado indicado no es válido.")]
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int? Estado { get; set; }

    /// <summary>URL para consultar la imagen del activo.</summary>
    /// <example>/imagenes/activos/ejemplo.png</example>
    [MaxLength(2048, ErrorMessage = "La URL de la imagen no puede superar los 2048 caracteres.")]
    public string? ImagenUrl { get; set; }

    /// <summary>Placa.</summary>
    /// <example>ABC-123</example>
    [MaxLength(20, ErrorMessage = "La placa no puede superar los 20 caracteres.")]
    public string? Placa { get; set; }

    /// <summary>Kilometraje registrado para el activo.</summary>
    /// <example>12000</example>
    [Required(ErrorMessage = "El kilometraje actual es obligatorio.")]
    [Range(
        0,
        int.MaxValue,
        ErrorMessage = "El kilometraje actual debe ser mayor o igual que cero.")]
    public int? KilometrajeActual { get; set; }

    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    [Required(ErrorMessage = "La categoría del activo es obligatoria.")]
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "La categoría del activo debe ser válida.")]
    public int? CategoriaActivoId { get; set; }
}
