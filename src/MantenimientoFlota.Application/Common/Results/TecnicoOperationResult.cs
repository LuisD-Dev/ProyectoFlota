using MantenimientoFlota.Application.DTOs.Tecnico;

namespace MantenimientoFlota.Application.Common.Results;

public enum TecnicoOperationStatus
{
    Succeeded,
    DoesNotExist,
    HasAssociatedSpecialties
}

public sealed record TecnicoOperationResult(
    TecnicoOperationStatus Status,
    TecnicoDto? Value = null);
