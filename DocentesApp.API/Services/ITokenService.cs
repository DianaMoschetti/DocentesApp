using DocentesApp.Application.DTOs.Auth;
using DocentesApp.Data.Identity;

namespace DocentesApp.API.Services
{
    public interface ITokenService
    {
        Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user);
    }
}
