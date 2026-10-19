namespace DocentesApp.Shared.DTOs.Reportes
{
    // Un curso de la vista por asignatura, con los docentes de esa asignatura en el curso
    public class CursoConDocentesDto
    {
        public int CursoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public List<DocenteActividadDto> Docentes { get; set; } = new();
    }
}
