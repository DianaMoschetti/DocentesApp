using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Cargos
{
    public class CargoDto
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public EspecificacionCargo DetalleCargo { get; set; }
        public Condicion Condicion { get; set; }
        public float PuntosBase { get; set; }
        public string? Observaciones { get; set; }
    }
}
