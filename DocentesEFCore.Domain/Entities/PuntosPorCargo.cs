using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class PuntosPorCargo
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        // Los puntos son iguales para Regular e Interino — Condicion no influye en el cálculo
        public decimal PuntosBase { get; set; }
    }
}
