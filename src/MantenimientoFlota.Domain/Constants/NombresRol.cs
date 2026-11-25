namespace MantenimientoFlota.Domain.Constants;

/// <summary>
/// Nombres exactos de los roles del catálogo. Deben coincidir con el claim "role" del token
/// y con los datos sembrados en CatalogSeeder. No es un enum.
/// </summary>
public static class NombresRol
{
    public const string Administrador = "Administrador";
    public const string CoordinadorTecnico = "Coordinador/Técnico";
    public const string SolicitanteConsulta = "Solicitante/Consulta";
}
