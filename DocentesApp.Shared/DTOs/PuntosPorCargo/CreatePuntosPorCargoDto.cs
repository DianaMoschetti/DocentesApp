using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class CreatePuntosPorCargoDto
    {
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser Cargo (CargoDocente)
        //public DenominacionCargo Denominacion { get; set; }
        //public TipoCargo TipoCargo { get; set; }
        public CargoDocente Cargo { get; set; }
        public decimal PuntosBase { get; set; }
    }
}
