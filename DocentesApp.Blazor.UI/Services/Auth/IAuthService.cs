using DocentesApp.Blazor.UI.Services.Base;

namespace DocentesApp.Blazor.UI.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto loginDto);
        Task LogoutAsync();
    }
}
