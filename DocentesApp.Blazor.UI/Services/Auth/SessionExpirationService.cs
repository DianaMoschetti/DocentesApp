namespace DocentesApp.Blazor.UI.Services.Auth
{
    // Servicio scoped (= uno por circuito de Blazor Server).
    // Centraliza el aviso de "sesión expirada / 401" para que no haya que tocar cada página.
    // Quien lo dispara: Client.ProcessResponse (ServiceClient.Partial.cs) cuando la API devuelve 401.
    // Quien lo escucha: SessionExpirationWatcher (en MainLayout), que redirige a /auth/signout.
    // No se inyecta NavigationManager en el DelegatingHandler porque los handlers de
    // IHttpClientFactory viven en otro scope de DI, no en el del circuito.
    public class SessionExpirationService
    {
        private int _notified;

        public event Action? SessionExpired;

        public bool IsExpired => _notified == 1;

        // Se puede llamar desde cualquier hilo; el evento se dispara una sola vez por circuito.
        public void NotifyUnauthorized()
        {
            if (Interlocked.Exchange(ref _notified, 1) == 0)
                SessionExpired?.Invoke();
        }
    }
}
