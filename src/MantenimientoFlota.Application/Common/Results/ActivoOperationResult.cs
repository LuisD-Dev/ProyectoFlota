using MantenimientoFlota.Application.DTOs.Activo;

namespace MantenimientoFlota.Application.Common.Results;

public enum ActivoOperationStatus
{
    Succeeded,
    DoesNotExist,
    CodeAlreadyExists,
    CategoryDoesNotExist
}

public sealed record ActivoOperationResult(
    ActivoOperationStatus Status,
    ActivoDto? Value = null);
