using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class PuntosPorCargo
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser CargoDocente (una fila por cargo). Se comentan en la etapa Data.
        [Obsolete("Usar Cargo.")]
        public DenominacionCargo Denominacion { get; set; }
        [Obsolete("Usar Cargo.")]
        public TipoCargo TipoCargo { get; set; }
        public CargoDocente Cargo { get; set; }
        // Los puntos son iguales para Regular e Interino — Condicion no influye en el cálculo
        public decimal PuntosBase { get; set; } // por 1 dedicación simple
    }
}
