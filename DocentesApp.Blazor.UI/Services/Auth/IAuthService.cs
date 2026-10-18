using DocentesApp.Blazor.UI.Services.Base;
using DocentesApp.Shared.DTOs.Auth;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public interface IAuthService
    {
        // errorMessage: el mensaje de la API cuando hay que mostrarlo (403 usuario inactivo); null = credenciales inválidas
        Task<(bool success, string? token, string? errorMessage)> LoginAsync(LoginDto loginDto);
        void SetPendingToken(string token);
        string? GetAndClearPendingToken();
        Task LogoutAsync();
    }
}