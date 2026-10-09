using DocentesApp.Blazor.UI.Services.Auth;

namespace DocentesApp.Blazor.UI.Services.Base
{
    // Extensión del cliente generado por NSwag (ServiceClient.cs).
    // Va en un archivo aparte para que no se pierda al regenerar el cliente.
    public partial class Client
    {
        // Se asigna en Program.cs al crear el IClient (scope del circuito).
        public SessionExpirationService? SessionExpiration { get; set; }

        // NSwag llama a este método parcial en cada respuesta, antes de lanzar ApiException.
        // Si la API devuelve 401 para un request que llevaba token, la sesión expiró:
        // avisamos al circuito para que redirija al login. La página igual recibe su
        // ApiException y la maneja con su catch habitual, así que no se cae el circuito.
        // (El login con credenciales inválidas también devuelve 401, pero ese request no lleva token.)
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                && response.RequestMessage?.Headers.Authorization != null)
            {
                SessionExpiration?.NotifyUnauthorized();
            }
        }
    }
}
