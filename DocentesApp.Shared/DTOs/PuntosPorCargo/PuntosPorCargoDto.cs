using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class PuntosPorCargoDto
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser Cargo (CargoDocente)
        //public DenominacionCargo Denominacion { get; set; }
        //public TipoCargo TipoCargo { get; set; }
        public CargoDocente Cargo { get; set; }
        public decimal PuntosBase { get; set; }
        // [Diana desde v4.0 OBSOLETO] antes "{Denominacion} {TipoCargo}"
        //public string Descripcion { get; set; } = string.Empty; // calculada en el mapeo: "{Denominacion} {TipoCargo}"
        public string Descripcion { get; set; } = string.Empty; // calculada en el mapeo: nombre del cargo (ej. "Profesor Adjunto")
    }
}
