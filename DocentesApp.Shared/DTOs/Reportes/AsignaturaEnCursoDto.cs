namespace DocentesApp.Shared.DTOs.Reportes
{
    // Una asignatura de un curso con sus docentes
    public class AsignaturaEnCursoDto
    {
        public int AsignaturaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        // Docentes con actividad en este curso
        public List<DocenteActividadDto> Docentes { get; set; } = new();
        // Jefe de Cátedra y Laboratorio: son de la asignatura, no de un curso
        public List<DocenteActividadDto> Catedra { get; set; } = new();
    }
}
