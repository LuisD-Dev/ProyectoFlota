using MantenimientoFlota.Application.DTOs.TecnicoEspecialidad;

namespace MantenimientoFlota.Application.Common.Results;

public enum TecnicoEspecialidadOperationStatus
{
    Succeeded,
    DoesNotExist,
    TechnicianDoesNotExist,
    SpecialtyDoesNotExist,
    RelationshipAlreadyExists
}

public sealed record TecnicoEspecialidadOperationResult(
    TecnicoEspecialidadOperationStatus Status,
    TecnicoEspecialidadDto? Value = null);
