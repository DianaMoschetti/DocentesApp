namespace DocentesApp.Shared.DTOs.Usuarios
{
    // El Administrador le pone una contraseña nueva a otro usuario (no necesita la actual)
    public class ResetPasswordDto
    {
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
