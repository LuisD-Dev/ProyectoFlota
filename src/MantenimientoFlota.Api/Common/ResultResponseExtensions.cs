using MantenimientoFlota.Domain.DTO.PatternResult;
using Microsoft.AspNetCore.Mvc;

namespace MantenimientoFlota.Api.Common;

public static class ResultResponseExtensions
{
    public static ActionResult FromResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (!result.IsSuccess)
            return controller.StatusCode(result.Status, ApiErrorFactory.Create(
                controller.HttpContext, result.Status, result.Detail, title: result.Title));
        return result.Status == StatusCodes.Status204NoContent
            ? controller.NoContent()
            : controller.StatusCode(result.Status, result);
    }
}
