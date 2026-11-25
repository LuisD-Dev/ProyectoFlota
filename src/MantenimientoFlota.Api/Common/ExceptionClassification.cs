using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MantenimientoFlota.Api.Common;

internal static class ExceptionClassification
{
    public static bool IsPersistenceConflict(Exception exception)
    {
        // EF puede envolver un deadlock en InvalidOperationException/RetryLimitExceededException.
        // Se inspeccionan los tipos y códigos, sin exponer sus mensajes al cliente.
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbUpdateConcurrencyException ||
                current is SqlException { Number: 547 or 1205 or 2601 or 2627 or 3960 })
                return true;
        }
        return false;
    }
}
