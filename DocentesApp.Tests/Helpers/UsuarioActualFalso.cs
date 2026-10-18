using DocentesApp.Application.Interfaces.Auth;

namespace DocentesApp.Tests.Helpers
{
    // Usuario actual fijo para los tests de auditoría. Sin UserId = "Sistema".
    public sealed class UsuarioActualFalso : ICurrentUserService
    {
        public string? UserId { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }

        public static UsuarioActualFalso Editor() => new() { UserId = "user-editor", Nombre = "Editora, Eva", Email = "editor@test.com" };

        public static UsuarioActualFalso Sistema() => new();
    }
}
