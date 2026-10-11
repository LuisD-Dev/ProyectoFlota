using MantenimientoFlota.Application.DTOs.RepuestoProveedor;

namespace MantenimientoFlota.Application.Common.Results;

public enum RepuestoProveedorOperationStatus
{
    Succeeded,
    DoesNotExist,
    RepuestoDoesNotExist,
    ProveedorDoesNotExist,
    AlreadyExists
}

public sealed record RepuestoProveedorOperationResult(
    RepuestoProveedorOperationStatus Status,
    RepuestoProveedorDto? Value = null);
