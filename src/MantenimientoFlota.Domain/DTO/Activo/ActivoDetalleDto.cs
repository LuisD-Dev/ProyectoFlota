using MantenimientoFlota.Domain.Constants;

namespace MantenimientoFlota.Domain.DTO.Activo;

/// <summary>Contrato de activo detalle.</summary>
public sealed class ActivoDetalleDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }
    /// <summary>Código identificador del recurso.</summary>
    /// <example>VEH-001</example>
    public string Codigo { get; set; } = string.Empty;
    /// <summary>Nombre.</summary>
    /// <example>Vehículo de servicio</example>
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Descripción del recurso.</summary>
    /// <example>Unidad de mantenimiento</example>
    public string Descripcion { get; set; } = string.Empty;
    /// <summary>Fecha de adquisición del activo.</summary>
    /// <example>2022-01-15</example>
    public DateOnly FechaAdquisicion { get; set; }
    /// <summary>Valor de referencia del activo.</summary>
    /// <example>1000000</example>
    public decimal ValorReferencia { get; set; }
    /// <summary>Estado numérico del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio y 3 DadoDeBaja.</summary>
    /// <example>0</example>
    [System.ComponentModel.Description("Estado del activo: 0 Operativo, 1 EnMantenimiento, 2 FueraDeServicio, 3 DadoDeBaja.")]
    public int Estado { get; set; }
    /// <summary>URL para consultar la imagen del activo.</summary>
    /// <example>/imagenes/activos/ejemplo.png</example>
    public string ImagenUrl { get; set; } = string.Empty;
    /// <summary>Placa.</summary>
    /// <example>ABC-123</example>
    public string Placa { get; set; } = string.Empty;
    /// <summary>Kilometraje registrado para el activo.</summary>
    /// <example>12000</example>
    public int KilometrajeActual { get; set; }
    /// <summary>Identificador del recurso asociado; debe corresponder a un registro existente.</summary>
    /// <example>1</example>
    public int CategoriaActivoId { get; set; }
    /// <summary>Nombre de la categoría asociada.</summary>
    public string CategoriaActivoNombre { get; set; } = string.Empty;
    /// <summary>Categoría asociada al activo.</summary>
    public CategoriaActivoResumenDto Categoria { get; set; } = new();
    /// <summary>Órdenes asociadas al activo.</summary>
    public IReadOnlyList<OrdenMantenimientoResumenDto> Ordenes { get; set; } =
        new List<OrdenMantenimientoResumenDto>();
    /// <summary>Número total de órdenes asociadas.</summary>
    public int CantidadOrdenes { get; set; }
    /// <summary>Antigüedad expresada en años completos.</summary>
    public int AntiguedadAnios { get; set; }
}
