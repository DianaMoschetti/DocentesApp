using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    public class PuntosPorCargoDto
    {
        public int Id { get; set; }
        public CargoDocente Cargo { get; set; }
        public decimal PuntosBase { get; set; }
        public string Descripcion { get; set; } = string.Empty; // calculada en el mapeo: nombre del cargo (ej. "Profesor Adjunto")
    }
}
