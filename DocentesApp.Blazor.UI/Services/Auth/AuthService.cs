using DocentesApp.Blazor.UI.Services.Base;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IClient _client;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(IClient client, CustomAuthStateProvider authStateProvider)
        {
            _client = client;
            _authStateProvider = authStateProvider;
        }
        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _client.LoginAsync(loginDto);
                if (response?.Token == null)
                    return false;

                _authStateProvider.NotifyUserLogin(response.Token);
                return true;
            }
            catch (ApiException)
            {
                return false;
            }
        }

        public Task LogoutAsync()
        {
            _authStateProvider.NotifyUserLogout();
            return Task.CompletedTask;
        }
    }
}
