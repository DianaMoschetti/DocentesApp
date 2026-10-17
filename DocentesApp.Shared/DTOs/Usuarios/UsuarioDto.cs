namespace DocentesApp.Shared.DTOs.Usuarios
{
    // Listado y detalle de usuarios (gestión de usuarios, solo Administrador)
    public class UsuarioDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string Rol { get; set; } = string.Empty; // un solo rol por usuario (Roles.Todos)
        public bool Activo { get; set; }
    }
}
