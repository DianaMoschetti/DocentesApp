using System.Net.Http.Headers;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    // intercepta las solicitudes HTTP salientes para agregar el token de autenticación al encabezado de la solicitud.
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        private readonly CustomAuthStateProvider _authStateProvider;
        public AuthorizationMessageHandler(CustomAuthStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        //se ejecuta antes de enviar la solicitud HTTP, agregando el token de autenticación al encabezado de la solicitud.
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _authStateProvider.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken); // agrega el token y sigue con el request normal
        }
    }
}
