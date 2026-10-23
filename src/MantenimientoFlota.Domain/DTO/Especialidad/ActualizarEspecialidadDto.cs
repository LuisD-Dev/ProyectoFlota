using System.ComponentModel.DataAnnotations;

namespace MantenimientoFlota.Domain.DTO.Especialidad;

/// <summary>Contrato de actualizar especialidad.</summary>
public sealed class ActualizarEspecialidadDto
{
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Descripción del recurso.</summary>
    /// <example>Unidad de mantenimiento</example>
    [MaxLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }
}
