using Microsoft.AspNetCore.Identity;

namespace DocentesApp.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Los usuarios no se borran (se conserva el historial): se desactivan.
        // Un usuario inactivo no puede loguearse y su token deja de valer (ver ValidacionTokenUsuario).
        public bool Activo { get; set; } = true;

        // IdentityUser ya tiene propiedades como UserName, Email, PasswordHash, etc.
    }
}
