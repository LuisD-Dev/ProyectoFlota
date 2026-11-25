namespace MantenimientoFlota.Domain.DTO.Usuario;

public sealed class UsuarioDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int RolId { get; set; }
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaCreacionUtc { get; set; }
    public DateTime? UltimoAccesoUtc { get; set; }
}

public sealed class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
