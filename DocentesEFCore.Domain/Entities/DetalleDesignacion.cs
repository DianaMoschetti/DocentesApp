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
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public Condicion Condicion { get; set; }
        public TipoDedicacion TipoDedicacion { get; set; }
        public float CantidadDedicacion { get; set; }
        public decimal PuntosAsignados { get; set; }
        public string? Observaciones { get; set; }
    }
}
