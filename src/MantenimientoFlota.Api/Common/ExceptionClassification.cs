using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MantenimientoFlota.Infrastructure.Exceptions;

namespace MantenimientoFlota.Api.Common;

internal static class ExceptionClassification
{
    public static bool IsPersistenceConflict(Exception exception)
    {
        if (exception is ImagenUpdateRejectedException { InnerException: not null } image)
            exception = image.InnerException!;
        return exception is DbUpdateConcurrencyException
            || exception is DbUpdateException { InnerException: SqlException sql }
                && sql.Number is 547 or 2601 or 2627;
    }
}
