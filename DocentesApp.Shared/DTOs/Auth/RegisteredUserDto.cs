namespace DocentesApp.Shared.DTOs.Auth
{
    // Respuesta del registro de usuarios (solo Admin): los datos del usuario creado, sin token.
    // El token es del usuario nuevo y no le sirve al admin que lo crea; el usuario lo obtiene al loguearse.
    public class RegisteredUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
