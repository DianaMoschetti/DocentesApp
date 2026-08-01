using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Cargos
{
    // [Diana desde v4.0 OBSOLETO] DTO de Cargo reemplazado por atributos en CreateDetalleDesignacionDto/DetalleDesignacionDto.
    // Mantener hasta completar el refactor completo.
    [Obsolete("DTO de Cargo obsoleto desde v4.0. Los atributos de cargo pasan a DetalleDesignacion.")]
    public class CargoDto
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public Condicion Condicion { get; set; }
        public float PuntosBase { get; set; }
        public string? Observaciones { get; set; }
        public string Descripcion { get; set; } = string.Empty; // campo "calculado" combino los otros 3 enums
    }
}
