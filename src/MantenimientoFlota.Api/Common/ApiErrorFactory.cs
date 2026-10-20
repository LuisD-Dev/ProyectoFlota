using MantenimientoFlota.Api.Contracts;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MantenimientoFlota.Api.Common;

public static class ApiErrorFactory
{
    public static ApiErrorResponse Create(
        HttpContext context,
        int status,
        string detail,
        Dictionary<string, string[]>? errors = null,
        string? title = null,
        string? errorCode = null)
    {
        if (status is StatusCodes.Status400BadRequest
            or StatusCodes.Status404NotFound
            or StatusCodes.Status409Conflict)
        {
            context.Items[RequestOutcomeMarker.Key] = RequestOutcome.ExpectedFailure;
        }

        var timestampUtc = DateTimeOffset.UtcNow;

        return new ApiErrorResponse(
            status,
            title ?? status switch
            {
                StatusCodes.Status400BadRequest => "Solicitud no válida",
                StatusCodes.Status404NotFound => "Recurso no encontrado",
                StatusCodes.Status409Conflict => "Conflicto",
                _ => "Error interno"
            },
            detail,
            context.TraceIdentifier,
            errors ?? new Dictionary<string, string[]>(),
            errorCode ?? status switch
            {
                StatusCodes.Status400BadRequest => ApiErrorCodes.InvalidRequest,
                StatusCodes.Status404NotFound => ApiErrorCodes.ResourceNotFound,
                StatusCodes.Status409Conflict => ApiErrorCodes.Conflict,
                _ => ApiErrorCodes.UnexpectedError
            },
            timestampUtc);
    }

    public static ApiErrorResponse Validation(HttpContext context, ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors.Select(error =>
                    error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "El valor enviado no es válido."
                        : error.ErrorMessage).ToArray());

        var response = Create(context, StatusCodes.Status400BadRequest,
            "Revise los campos indicados.", errors, "Error de validación",
            errorCode: ApiErrorCodes.ValidationError);
        context.Items[RequestOutcomeMarker.Key] = RequestOutcome.Validation;
        return response;
    }
}
