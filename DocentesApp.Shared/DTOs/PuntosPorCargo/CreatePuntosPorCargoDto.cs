using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class CreatePuntosPorCargoDto
    {
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser Cargo (CargoDocente). Se comentan cuando se reescriba PuntosPorCargoForm.
        [Obsolete("Usar Cargo.")]
        public DenominacionCargo Denominacion { get; set; }
        [Obsolete("Usar Cargo.")]
        public TipoCargo TipoCargo { get; set; }
        public CargoDocente Cargo { get; set; }
        public decimal PuntosBase { get; set; }
    }
}
