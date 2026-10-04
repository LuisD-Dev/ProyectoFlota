using MantenimientoFlota.Application.DTOs.Repuesto;

namespace MantenimientoFlota.Application.Common.Results;

public enum RepuestoOperationStatus
{
    Succeeded,
    DoesNotExist,
    HasAssociatedSuppliers
}

public sealed record RepuestoOperationResult(
    RepuestoOperationStatus Status,
    RepuestoDto? Value = null);
