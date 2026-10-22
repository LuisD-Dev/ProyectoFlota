namespace MantenimientoFlota.Domain.Entities;

public sealed class Usuario
{
    public int Id { get; set; }
    public int RolId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacionUtc { get; set; }
    public DateTime? UltimoAccesoUtc { get; set; }
    public Rol Rol { get; set; } = null!;
}
