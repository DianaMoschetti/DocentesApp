using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class PuntosPorCargo : BaseDomainModel
    {
        public int Id { get; set; }
        public CargoDocente Cargo { get; set; }
        // Los puntos son iguales para Regular e Interino — Condicion no influye en el cálculo
        public decimal PuntosBase { get; set; } // por 1 dedicación simple
    }
}
