namespace DocentesApp.Shared.DTOs.Usuarios
{
    // "Cambiar mi contraseña": cualquier usuario logueado, con su contraseña actual
    public class CambiarMiPasswordDto
    {
        public string PasswordActual { get; set; } = string.Empty;
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
