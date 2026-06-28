using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Cargos
{
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
