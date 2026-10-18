using DocentesApp.Domain.Base;
using Microsoft.AspNetCore.Identity;

namespace DocentesApp.Data.Identity
{
    // ahora implementa IAuditable (no puede heredar de BaseDomainModel porque hereda de IdentityUser)
    public class ApplicationUser : IdentityUser, IAuditable
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        // Los usuarios no se borran (se conserva el historial): se desactivan.
        // Un usuario inactivo no puede loguearse y su token deja de valer (ver ValidacionTokenUsuario).
        public bool Activo { get; set; } = true;

        // Auditoría (IAuditable): las completa el interceptor de auditoría. Usuarios anteriores: 0001-01-01 / NULL.
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }

        // IdentityUser ya tiene propiedades como UserName, Email, PasswordHash, etc.
    }
}
