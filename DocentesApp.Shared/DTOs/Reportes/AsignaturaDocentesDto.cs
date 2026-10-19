namespace DocentesApp.Shared.DTOs.Reportes
{
    // Vista por asignatura: todos sus cursos con los docentes de cargos vigentes
    public class AsignaturaDocentesDto
    {
        public int AsignaturaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreUdb { get; set; } = string.Empty;
        // Jefe de Cátedra y Laboratorio
        public List<DocenteActividadDto> Catedra { get; set; } = new();
        public List<CursoConDocentesDto> Cursos { get; set; } = new();
    }
}
