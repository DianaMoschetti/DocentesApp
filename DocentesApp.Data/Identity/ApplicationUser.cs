using Microsoft.AspNetCore.Identity;

namespace DocentesApp.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // IdentityUser ya tiene propiedades como UserName, Email, PasswordHash, etc.
    }
}
