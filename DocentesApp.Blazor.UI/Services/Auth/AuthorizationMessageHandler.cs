using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    // lee el token del claim "access_token" que se agregó a la identidad del usuario durante el login,
    // y lo agrega al encabezado de autorización de cada solicitud HTTP saliente.
    // Si la respuesta es 401 (sesión expirada), redirige al login automáticamente.
    // [Diana desde v4.0] El 401 ya no se maneja acá: este handler corre en el scope de
    // IHttpClientFactory (no en el del circuito), así que el NavigationManager que obtenía
    // nunca estaba inicializado y la redirección fallaba en silencio.
    // Ahora lo detecta Client.ProcessResponse (ServiceClient.Partial.cs) y avisa a
    // SessionExpirationService, que es scoped al circuito.
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationMessageHandler(
            IHttpContextAccessor httpContextAccessor
            )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _httpContextAccessor.HttpContext?.User?
                .FindFirst("access_token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);

            return response;
        }
    }
}
