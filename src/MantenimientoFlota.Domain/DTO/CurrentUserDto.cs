namespace MantenimientoFlota.Domain.DTO;

/// <summary>Datos del usuario leídos de los claims del token.</summary>
public record class CurrentUserDto
{
    public string? Id { get; set; }
    public string? Login { get; set; }
    public string? NombreCompleto { get; set; }
    public string? Rol { get; set; }
}