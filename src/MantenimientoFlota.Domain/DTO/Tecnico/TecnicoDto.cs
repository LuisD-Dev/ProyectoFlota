namespace MantenimientoFlota.Domain.DTO.Tecnico;

/// <summary>Contrato de tecnico.</summary>
public sealed class TecnicoDto
{
    /// <summary>Identificador asignado al recurso.</summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>Nombre completo del técnico.</summary>
    /// <example>María Rodríguez</example>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>Identificación del técnico.</summary>
    /// <example>ID-DEMO-001</example>
    public string Identificacion { get; set; } = string.Empty;

    /// <summary>Número de teléfono de contacto.</summary>
    /// <example>8888-0000</example>
    public string Telefono { get; set; } = string.Empty;

    /// <summary>Correo electrónico de contacto.</summary>
    /// <example>demo@example.test</example>
    public string Correo { get; set; } = string.Empty;

    /// <summary>Indica si el técnico está disponible.</summary>
    /// <example>true</example>
    public bool Disponible { get; set; }

    /// <summary>Tarifa del técnico por hora.</summary>
    /// <example>15000</example>
    public decimal TarifaPorHora { get; set; }
}
