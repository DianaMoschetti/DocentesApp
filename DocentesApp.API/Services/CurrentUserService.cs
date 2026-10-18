using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DocentesApp.Application.Interfaces.Auth;

namespace DocentesApp.API.Services
{
    // Usuario actual para la auditoría, leído de los claims del token (sin consultar la base).
    // Sin HttpContext o sin usuario autenticado (seed, login) devuelve null: el interceptor registra "Sistema".
    public class CurrentUserService : ICurrentUserService
    {
        public const string ClaimNombre = "nombre";     // lo emite TokenService: "Apellido, Nombre"
        public const string ClaimUserName = "userName"; // fallback para tokens emitidos antes del claim "nombre"

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Usuario
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.Identity?.IsAuthenticated == true ? user : null;
            }
        }

        public string? UserId => Valor(ValidacionTokenUsuario.ClaimUserId);

        public string? Nombre => Valor(ClaimNombre) ?? Valor(ClaimUserName);

        // JwtBearer mapea "email" a ClaimTypes.Email; se aceptan los dos
        public string? Email => Valor(ClaimTypes.Email) ?? Valor(JwtRegisteredClaimNames.Email);

        private string? Valor(string claim)
        {
            var valor = Usuario?.FindFirst(claim)?.Value;
            return string.IsNullOrWhiteSpace(valor) ? null : valor;
        }
    }
}
