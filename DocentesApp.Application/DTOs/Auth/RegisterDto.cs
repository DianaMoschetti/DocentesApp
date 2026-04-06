
namespace DocentesApp.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // Diana, ver de sacar el user name y q se use el mail para loguearse
        public string Password { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}
