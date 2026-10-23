using System.ComponentModel.DataAnnotations;
using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de consultar activos.</summary>
public sealed class ConsultarActivosDto
{
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    [Range(1, int.MaxValue,
        ErrorMessage = "El identificador de la categoría debe ser mayor que cero.")]
    public int? CategoriaActivoId { get; set; }

    /// <summary>Estado numérico del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio y 3 DadoDeBaja.</summary>
    /// <example>0</example>
    [Range(EstadoActivo.Operativo, EstadoActivo.DadoDeBaja, ErrorMessage = "El estado indicado no es válido.")]
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int? Estado { get; set; }
}
