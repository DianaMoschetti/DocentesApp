namespace DocentesApp.Shared.DTOs.Usuarios
{
    // Alta de usuario por el Administrador, con su rol (reemplaza a RegisterDto + assign-role)
    public class CreateUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}
