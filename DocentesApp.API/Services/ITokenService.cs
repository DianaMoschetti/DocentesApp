using DocentesApp.Data.Identity;
using DocentesApp.Shared.DTOs.Auth;

namespace DocentesApp.API.Services
{
    public interface ITokenService
    {
        Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user);
    }
}
