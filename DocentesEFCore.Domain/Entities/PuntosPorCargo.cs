using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class PuntosPorCargo
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        public Condicion Condicion { get; set; }
        public decimal PuntosBase { get; set; }
    }
}
