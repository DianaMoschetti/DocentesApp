using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Reportes
{
    // Vista por curso: un curso (comisión) con sus asignaturas y los docentes de cargos vigentes
    public class CursoDocentesDto
    {
        public int CursoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;         // formato actual del Curso
        public Especialidad Carrera { get; set; }
        public Nivel Año { get; set; }
        public int NroComision { get; set; }
        public Turno Turno { get; set; }
        public List<AsignaturaEnCursoDto> Asignaturas { get; set; } = new();
    }
}
