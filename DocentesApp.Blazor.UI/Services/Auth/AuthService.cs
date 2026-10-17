using DocentesApp.Blazor.UI.Helpers;
using DocentesApp.Blazor.UI.Services.Auth;
using DocentesApp.Blazor.UI.Services.Base;
using DocentesApp.Shared.DTOs.Auth;

public class AuthService : IAuthService
{
    private readonly IClient _client;
    private string? _pendingToken;

    // solo lo uso para guardar el token, no llamo al endpoint desde aca
    public AuthService(IClient client)
    {
        _client = client;
    }

    // [Diana desde v4.0 OBSOLETO] se agrega el mensaje de error de la API (usuario inactivo)
    //public async Task<(bool success, string? token)> LoginAsync(LoginDto loginDto)
    //{
    //    try
    //    {
    //        var response = await _client.LoginAsync(loginDto);
    //        if (response?.Token == null)
    //            return (false, null);
    //        return (true, response.Token);
    //    }
    //    catch (ApiException)
    //    {
    //        return (false, null);
    //    }
    //}
    public async Task<(bool success, string? token, string? errorMessage)> LoginAsync(LoginDto loginDto)
    {
        try
        {
            var response = await _client.LoginAsync(loginDto);
            if (response?.Token == null)
                return (false, null, null);
            return (true, response.Token, null);
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            // contraseña correcta pero usuario inactivo: se muestra el mensaje de la API
            return (false, null, ApiErrorHelper.GetMessage(ex, "Usuario inactivo. Contactá al administrador."));
        }
        catch (ApiException)
        {
            return (false, null, null); // 401: credenciales inválidas
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