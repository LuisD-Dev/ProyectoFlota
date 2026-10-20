namespace MantenimientoFlota.Api.Common;

internal static class ActivoImagenValidator
{
    internal const int MaxBytes = 2 * 1024 * 1024;

    internal static async Task<(MemoryStream? Contenido, string? Extension, string? Error)> LeerAsync(
        IFormFile archivo, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png"))
            return (null, null, "Solo se permiten imágenes JPEG y PNG.");
        if (archivo.Length <= 0 || archivo.Length > MaxBytes)
            return (null, null, "La imagen debe contener datos y no superar 2 MiB.");

        var contenido = new MemoryStream();
        try
        {
            await using var entrada = archivo.OpenReadStream();
            var buffer = new byte[81920];
            int count;
            while ((count = await entrada.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (contenido.Length + count > MaxBytes)
                {
                    contenido.Dispose();
                    return (null, null, "La imagen no puede superar 2 MiB.");
                }
                contenido.Write(buffer, 0, count);
            }

            var bytes = contenido.GetBuffer().AsSpan(0, (int)contenido.Length);
            var png = bytes.StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
            var jpeg = bytes.StartsWith(new byte[] { 255, 216, 255 });
            if ((extension == ".png" && !png) || (extension != ".png" && !jpeg))
            {
                contenido.Dispose();
                return (null, null, "El contenido del archivo no coincide con el formato de imagen indicado.");
            }
            contenido.Position = 0;
            return (contenido, png ? ".png" : ".jpg", null);
        }
        catch
        {
            contenido.Dispose();
            throw;
        }
    }
}
