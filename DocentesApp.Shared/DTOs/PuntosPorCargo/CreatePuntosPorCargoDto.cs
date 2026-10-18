using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class CreatePuntosPorCargoDto
    {
        public CargoDocente Cargo { get; set; }
        public decimal PuntosBase { get; set; }
    }
}
