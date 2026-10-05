using MantenimientoFlota.Application.DTOs.Proveedor;

namespace MantenimientoFlota.Application.Common.Results;

public enum ProveedorOperationStatus
{
    Succeeded,
    DoesNotExist,
    HasAssociatedParts
}

public sealed record ProveedorOperationResult(
    ProveedorOperationStatus Status,
    ProveedorDto? Value = null);
