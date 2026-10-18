namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Propiedad cambiada: etiqueta y valores para mostrar (null = sin valor; la UI muestra "—")
    public class HistorialCambioDetalleDto
    {
        public string Propiedad { get; set; } = string.Empty;
        public string? Anterior { get; set; }
        public string? Nuevo { get; set; }
    }
}
