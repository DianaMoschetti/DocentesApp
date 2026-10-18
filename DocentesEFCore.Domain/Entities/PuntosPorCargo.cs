using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    // [Diana desde v4.0 OBSOLETO] ahora hereda de BaseDomainModel (auditoría)
    //public class PuntosPorCargo
    public class PuntosPorCargo : BaseDomainModel
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser CargoDocente (una fila por cargo)
        //public DenominacionCargo Denominacion { get; set; }
        //public TipoCargo TipoCargo { get; set; }
        public CargoDocente Cargo { get; set; }
        // Los puntos son iguales para Regular e Interino — Condicion no influye en el cálculo
        public decimal PuntosBase { get; set; } // por 1 dedicación simple
    }
}
