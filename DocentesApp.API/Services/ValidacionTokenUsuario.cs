using DocentesApp.Data.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

namespace DocentesApp.API.Services
{
    /// <summary>
    /// Se ejecuta en cada request autenticado (JwtBearerEvents.OnTokenValidated), después de validar firma y vencimiento.
    /// Corta el token si el usuario ya no existe, está inactivo o su security stamp cambió
    /// (desactivación, cambio de rol, reseteo o cambio de contraseña): la API responde 401
    /// y Blazor manda al login (SessionExpirationService), sin esperar a que venza el token.
    /// Costo: una búsqueda por clave primaria en AspNetUsers por request.
    /// </summary>
    public static class ValidacionTokenUsuario
    {
        public const string ClaimUserId = "userId";        // mismo claim que emite TokenService
        public const string ClaimSecurityStamp = "sstamp";

        public static async Task ValidarAsync(TokenValidatedContext context)
        {
            var userId = context.Principal?.FindFirst(ClaimUserId)?.Value;
            var stamp = context.Principal?.FindFirst(ClaimSecurityStamp)?.Value;

            // Tokens emitidos antes de este cambio no tienen stamp: se piden de nuevo (nuevo login)
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(stamp))
            {
                context.Fail("Token sin usuario o sin security stamp.");
                return;
            }

            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);

            if (user == null || !user.Activo || user.SecurityStamp != stamp)
                context.Fail("El usuario no existe, está inactivo o sus credenciales cambiaron.");
        }
    }
}
