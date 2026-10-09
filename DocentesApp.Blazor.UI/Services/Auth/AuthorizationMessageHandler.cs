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
        // [Diana desde v4.0 OBSOLETO]
        // private readonly IServiceProvider _serviceProvider;

        public AuthorizationMessageHandler(
            IHttpContextAccessor httpContextAccessor
            // [Diana desde v4.0 OBSOLETO]
            // , IServiceProvider serviceProvider
            )
        {
            _httpContextAccessor = httpContextAccessor;
            // [Diana desde v4.0 OBSOLETO]
            // _serviceProvider = serviceProvider;
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

            // Si la sesión expiró, redirigir al login
            // [Diana desde v4.0 OBSOLETO]
            // if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            // {
            //     try
            //     {
            //         using var scope = _serviceProvider.CreateScope();
            //         var navigationManager = scope.ServiceProvider
            //             .GetRequiredService<NavigationManager>();
            //         navigationManager.NavigateTo("/login?expired=true", forceLoad: true);
            //     }
            //     catch
            //     {
            //         // Si no se puede redirigir, dejamos que el error se propague normalmente
            //     }
            // }

            return response;
        }
    }
}
