using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public class CustomAuthStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        // Tolerancia por diferencia de reloj entre la UI y la API.
        private static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(30);

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthStateProvider(
            ILoggerFactory loggerFactory,
            IHttpContextAccessor httpContextAccessor) : base(loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // [Diana desde v4.0 OBSOLETO]
        // protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

        // [Diana desde v4.0 OBSOLETO]
        // protected override Task<bool> ValidateAuthenticationStateAsync(
        //     AuthenticationState authenticationState,
        //     CancellationToken cancellationToken)
        // {
        //     return Task.FromResult(true);
        // }

        // Dentro del circuito no hay requests HTTP nuevos, así que la cookie nunca se re-evalúa.
        // Por eso se revisa el "exp" del JWT guardado en el claim "access_token".
        // Si devuelve false, la clase base pasa el usuario a anónimo y dispara
        // AuthenticationStateChanged (lo escucha SessionExpirationWatcher).
        protected override Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState,
            CancellationToken cancellationToken)
        {
            var token = authenticationState.User.FindFirst("access_token")?.Value;
            if (string.IsNullOrEmpty(token))
                return Task.FromResult(false);

            try
            {
                // Solo lectura: la firma la valida la API.
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var vigente = jwt.ValidTo.Add(ClockSkew) > DateTime.UtcNow;
                return Task.FromResult(vigente);
            }
            catch (ArgumentException)
            {
                return Task.FromResult(false);
            }
        }
    }
}
