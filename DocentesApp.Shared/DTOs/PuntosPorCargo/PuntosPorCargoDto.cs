using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class PuntosPorCargoDto
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public decimal PuntosBase { get; set; }
        public string Descripcion { get; set; } = string.Empty; // calculada en el mapeo: "{Denominacion} {TipoCargo}"
    }
}
