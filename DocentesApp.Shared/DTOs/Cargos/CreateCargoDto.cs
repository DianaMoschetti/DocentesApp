using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Cargos
{
    public class CreateCargoDto
    {
        public DenominacionCargo Denominacion { get; set; }
        public TipoCargo TipoCargo { get; set; }
        
        public Condicion Condicion { get; set; } // Interino, ordinario, regular, suplente, etc.
        public float PuntosBase { get; set; }
        public string? Observaciones { get; set; }
    }
}