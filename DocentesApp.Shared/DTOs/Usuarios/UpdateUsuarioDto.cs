namespace DocentesApp.Shared.DTOs.Usuarios
{
    // Edición de los datos personales: el email no se edita
    public class UpdateUsuarioDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}
