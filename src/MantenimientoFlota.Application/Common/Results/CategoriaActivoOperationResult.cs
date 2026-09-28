using MantenimientoFlota.Application.DTOs.CategoriaActivo;

namespace MantenimientoFlota.Application.Common.Results;

public enum CategoriaActivoOperationStatus
{
    Succeeded,
    DoesNotExist,
    NameAlreadyExists,
    HasAssociatedAssets
}

public sealed record CategoriaActivoOperationResult(
    CategoriaActivoOperationStatus Status,
    CategoriaActivoDto? Value = null);
