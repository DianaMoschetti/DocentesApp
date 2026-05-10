using DocentesApp.Blazor.UI.Services.Auth;
using DocentesApp.Blazor.UI.Services.Base;

public class AuthService : IAuthService
{
    private readonly IClient _client;
    private string? _pendingToken;

    // solo lo uso para guardar el token, no llamo al endpoint desde aca
    public AuthService(IClient client)
    {
        _client = client;
    }

    public async Task<(bool success, string? token)> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var response = await _client.LoginAsync(loginDto);
            if (response?.Token == null)
                return (false, null);
            return (true, response.Token);
        }
        catch (ApiException)
        {
            return (false, null);
        }
    }

    public string? GetAndClearPendingToken()
    {
        var token = _pendingToken;
        _pendingToken = null;
        return token;
    }

    public void SetPendingToken(string token) => _pendingToken = token;

    public async Task LogoutAsync()
    {
       
    }
}