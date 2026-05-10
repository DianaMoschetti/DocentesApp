using DocentesApp.Blazor.UI.Services.Base;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public interface IAuthService
    {
        Task<(bool success, string? token)> LoginAsync(LoginDto loginDto);
        void SetPendingToken(string token);
        string? GetAndClearPendingToken();
        Task LogoutAsync();
    }
}