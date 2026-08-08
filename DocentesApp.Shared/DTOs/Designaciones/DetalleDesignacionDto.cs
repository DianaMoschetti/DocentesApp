using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class DetalleDesignacionDto
    {
        public int Id { get; set; }
        public int DesignacionId { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public Condicion Condicion { get; set; }
        public TipoDedicacion TipoDedicacion { get; set; }
        public float CantidadDedicacion { get; set; }
        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; }
        public string? NombreAsignatura { get; set; }
        public int? CursoId { get; set; }
        public string? DescripcionCurso { get; set; }
        // [Diana desde v4.0 OBSOLETO] PuntosUtilizados renombrado a PuntosAsignados
        // public decimal? PuntosUtilizados { get; set; }
        public decimal? PuntosAsignados { get; set; }
        public string? Observaciones { get; set; }
    }
}