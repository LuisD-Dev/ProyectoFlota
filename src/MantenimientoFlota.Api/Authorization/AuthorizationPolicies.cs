namespace MantenimientoFlota.Api.Authorization;

/// <summary>
/// Nombres de las políticas de autorización (matriz de permisos de la sección 11 del enunciado).
/// Se registran en Program.cs y se aplican por acción con [Authorize(Policy = ...)].
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>CRUD de catálogos, activos y cambios administrativos: solo Administrador.</summary>
    public const string SoloAdministrador = "SoloAdministrador";

    /// <summary>Consulta de categorías, técnicos, especialidades, repuestos y proveedores: Administrador y Coordinador/Técnico.</summary>
    public const string AdministradorOCoordinador = "AdministradorOCoordinador";

    /// <summary>Consulta de activos y de órdenes: los tres roles (el Solicitante solo ve sus propias órdenes, regla del servicio).</summary>
    public const string TodosLosRoles = "TodosLosRoles";

    /// <summary>Crear solicitudes de mantenimiento: Administrador y Solicitante/Consulta.</summary>
    public const string CrearSolicitudes = "CrearSolicitudes";
}
