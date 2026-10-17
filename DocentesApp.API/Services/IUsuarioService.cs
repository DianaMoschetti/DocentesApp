using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Usuarios;

namespace DocentesApp.API.Services
{
    // Gestión de usuarios (Identity). actorId = Id del usuario logueado que hace la operación (claim "userId").
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto>> GetAllAsync();
        Task<UsuarioDto> GetByIdAsync(string id);
        Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
        Task UpdateAsync(string id, UpdateUsuarioDto dto);
        Task CambiarRolAsync(string id, string rol, string actorId);
        Task DesactivarAsync(string id, string actorId);
        Task ReactivarAsync(string id);
        Task ResetPasswordAsync(string id, string passwordNueva);
        // Devuelve un token nuevo: el cambio de contraseña cambia el security stamp y el token actual deja de valer
        Task<AuthResponseDto> CambiarMiPasswordAsync(string actorId, CambiarMiPasswordDto dto);
    }
}
