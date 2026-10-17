using Microsoft.AspNetCore.Authorization;

namespace DocentesApp.Shared.Auth
{
    /// <summary>
    /// Políticas de autorización. La API y Blazor las registran con <see cref="Configurar"/>,
    /// así las dos usan exactamente las mismas reglas.
    /// Los controllers y las páginas usan el nombre de la política, nunca strings de rol:
    /// si más adelante se restringe por UDB, se agrega un requirement acá sin tocar los controllers.
    /// </summary>
    public static class Politicas
    {
        public const string PuedeConsultar = "PuedeConsultar";   // cualquier usuario con rol (lecturas y reportes)
        public const string PuedeEditar = "PuedeEditar";         // altas, modificaciones y bajas de datos y catálogos
        public const string EsAdministrador = "EsAdministrador"; // gestión de usuarios (y, más adelante, auditoría)

        // Solo registra las políticas con nombre. FallbackPolicy/DefaultPolicy las fija cada host:
        // la API usa PuedeConsultar; Blazor no, porque bloquearía /login y /auth/signin (anónimos).
        public static void Configurar(AuthorizationOptions options)
        {
            options.AddPolicy(PuedeConsultar, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(Roles.Administrador, Roles.Editor, Roles.Consultante));

            options.AddPolicy(PuedeEditar, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(Roles.Administrador, Roles.Editor));

            options.AddPolicy(EsAdministrador, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(Roles.Administrador));
        }
    }
}
