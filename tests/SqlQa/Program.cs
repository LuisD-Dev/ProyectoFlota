using Microsoft.Data.SqlClient;

// Auxiliar exclusivo de pruebas. No recibe ni imprime contraseñas por argumentos.
var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("FLOTA_QA_CONNECTION"));
if (!builder.InitialCatalog.StartsWith("MantenimientoFlotaDb_QA_Belen_", StringComparison.Ordinal))
    throw new InvalidOperationException("Este auxiliar solo puede usar una base aislada de QA.");
var sql = args.SingleOrDefault() switch
{
    "fallar-consumos" => "CREATE TRIGGER dbo.QA_FalloRepuestos ON dbo.RepuestosOrden AFTER INSERT AS BEGIN THROW 51000, 'Fallo controlado QA', 1; END;",
    "limpiar-fallo" => "DROP TRIGGER IF EXISTS dbo.QA_FalloRepuestos;",
    _ => throw new InvalidOperationException("Operación QA desconocida.")
};
await using var connection = new SqlConnection(builder.ConnectionString);
await connection.OpenAsync();
await using var command = connection.CreateCommand();
command.CommandText = sql;
await command.ExecuteNonQueryAsync();
Console.WriteLine("Operación QA completada en base aislada.");
