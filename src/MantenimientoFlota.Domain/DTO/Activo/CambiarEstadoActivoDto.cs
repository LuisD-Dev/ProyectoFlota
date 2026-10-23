using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de cambiar estado activo.</summary>
public sealed class CambiarEstadoActivoDto
{
    /// <summary>Estado numérico del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio y 3 DadoDeBaja.</summary>
    /// <example>0</example>
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [Range(EstadoActivo.Operativo, EstadoActivo.DadoDeBaja, ErrorMessage = "El estado indicado no es válido.")]
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int? Estado { get; set; }
}
