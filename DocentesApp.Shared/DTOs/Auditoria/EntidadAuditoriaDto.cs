namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Opción del filtro por entidad: clave (para el filtro) y etiqueta (para mostrar)
    public class EntidadAuditoriaDto
    {
        public string Entidad { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
