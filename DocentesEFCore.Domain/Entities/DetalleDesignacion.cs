using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class DetalleDesignacion
    {
        public int Id { get; set; }
        public int DesignacionId { get; set; }
        public Designacion Designacion { get; set; } = null!;
        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; }
        public Asignatura? Asignatura { get; set; }
        public int? CursoId { get; set; }
        public Curso? Curso { get; set; }
        public decimal? PuntosUtilizados { get; set; }
    }
}