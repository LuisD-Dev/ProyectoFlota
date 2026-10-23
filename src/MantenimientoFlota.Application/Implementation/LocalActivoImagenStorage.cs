using System.Globalization;
using System.Text.RegularExpressions;
using MantenimientoFlota.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MantenimientoFlota.Application.Implementation;

public sealed class LocalActivoImagenStorage(
    ActivoImagenStorageOptions options,
    ILogger<LocalActivoImagenStorage> logger) : IActivoImagenStorage
{
    public const string RequestPath = "/imagenes/activos";

    private static string CanonicalPath(string ruta)
    {
        var texto = ruta.Trim().Replace('\\', '/');
        if (Uri.TryCreate(texto, UriKind.Absolute, out var uri)) texto = uri.AbsolutePath;
        else texto = texto.Split('?', '#')[0];
        return Uri.UnescapeDataString(texto).Replace('\\', '/');
    }

    public bool EsRutaReservada(string? ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta)) return false;
        var path = CanonicalPath(ruta);
        // Resolver segmentos evita eludir la reserva mediante /otra/../imagenes/activos.
        var segments = new List<string>();
        foreach (var segment in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (segment == ".") continue;
            if (segment == "..") { if (segments.Count > 0) segments.RemoveAt(segments.Count - 1); }
            else segments.Add(segment);
        }
        var normalized = "/" + string.Join('/', segments);
        return normalized.Equals(RequestPath, StringComparison.OrdinalIgnoreCase)
            || normalized.StartsWith(RequestPath + "/", StringComparison.OrdinalIgnoreCase);
    }

    public bool EsRutaAdministrada(int activoId, string ruta)
    {
        // Solo las rutas exactas generadas por este almacenamiento otorgan permiso de borrado.
        return Regex.IsMatch(ruta,
            @"\A/imagenes/activos/" + activoId.ToString(CultureInfo.InvariantCulture)
            + @"/[0-9a-f]{32}\.(?:jpg|png)\z", RegexOptions.CultureInvariant);
    }

    private string PhysicalPath(int id, string fileName)
    {
        var root = Path.GetFullPath(options.RootPath).TrimEnd(Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        var path = Path.GetFullPath(Path.Combine(root, id.ToString(CultureInfo.InvariantCulture), fileName));
        if (!path.StartsWith(root, StringComparison.Ordinal)) throw new InvalidOperationException("Ruta de imagen inválida.");
        return path;
    }

    public async Task<string> GuardarAsync(int activoId, Stream contenido, string extension, CancellationToken cancellationToken)
    {
        if (extension is not (".jpg" or ".png") || activoId <= 0)
            throw new ArgumentException("Formato de imagen inválido.");

        var name = Guid.NewGuid().ToString("N") + extension;
        var path = PhysicalPath(activoId, name);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var created = false;
        try
        {
            await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            created = true;
            var buffer = new byte[81920];
            long total = 0;
            int count;
            while ((count = await contenido.ReadAsync(buffer, cancellationToken)) > 0)
            {
                total += count;
                if (total > 2 * 1024 * 1024) throw new InvalidDataException("La imagen supera el límite permitido.");
                await output.WriteAsync(buffer.AsMemory(0, count), cancellationToken);
            }
            return $"{RequestPath}/{activoId.ToString(CultureInfo.InvariantCulture)}/{name}";
        }
        catch
        {
            if (created) await EliminarSiAdministradaAsync(activoId,
                $"{RequestPath}/{activoId.ToString(CultureInfo.InvariantCulture)}/{name}");
            throw;
        }
    }

    public Task EliminarSiAdministradaAsync(int activoId, string ruta)
    {
        if (!EsRutaAdministrada(activoId, ruta)) return Task.CompletedTask;
        try { File.Delete(PhysicalPath(activoId, Path.GetFileName(ruta))); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning("No se pudo retirar una imagen administrada. ActivoId={ActivoId} ExceptionType={ExceptionType}",
                activoId, ex.GetType().FullName);
        }
        return Task.CompletedTask;
    }
}
