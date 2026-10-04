namespace MantenimientoFlota.Application.DTOs.Tecnico;

public sealed class TecnicoDto
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Identificacion { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public bool Disponible { get; set; }

    public decimal TarifaPorHora { get; set; }
}
