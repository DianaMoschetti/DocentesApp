using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class ActividadDto
    {
        public int Id { get; set; }
        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; }
        public string? NombreAsignatura { get; set; }
        public int? CursoId { get; set; }
        public string? DescripcionCurso { get; set; }
        public string? Horario { get; set; }
        public string? Observaciones { get; set; }
    }
}
