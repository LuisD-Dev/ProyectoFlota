using MantenimientoFlota.Application.DTOs.OrdenMantenimiento;

namespace MantenimientoFlota.Application.Common.Results;

public enum OrdenMantenimientoOperationStatus
{
    Succeeded,
    ActivoDoesNotExist
}

public sealed record OrdenMantenimientoOperationResult(
    OrdenMantenimientoOperationStatus Status,
    OrdenMantenimientoDto? Value = null);
