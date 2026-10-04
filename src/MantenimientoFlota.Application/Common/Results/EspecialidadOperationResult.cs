using MantenimientoFlota.Application.DTOs.Especialidad;

namespace MantenimientoFlota.Application.Common.Results;

public enum EspecialidadOperationStatus
{
    Succeeded,
    DoesNotExist,
    NameAlreadyExists,
    HasAssociatedTechnicians
}

public sealed record EspecialidadOperationResult(
    EspecialidadOperationStatus Status,
    EspecialidadDto? Value = null);
