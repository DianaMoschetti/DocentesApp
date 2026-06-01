
using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class CreateDetalleDesignacionDto
    {
        // sin id pq se genera cuando se intesrta en la db
        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; }
        public int? CursoId { get; set; }
        public decimal? PuntosUtilizados { get; set; }
    }
}
