using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class CargoDesignacionDto
    {
        public int Id { get; set; }
        public CargoDocente Cargo { get; set; }
        public string CargoTexto { get; set; } = string.Empty;      // "Profesor Adjunto"
        public string CargoCorto { get; set; } = string.Empty;      // "PAd"
        public Condicion Condicion { get; set; }
        public string CondicionCorta { get; set; } = string.Empty;  // "Int"
        public TipoDedicacion TipoDedicacion { get; set; }
        public decimal CantidadDedicacion { get; set; }
        public string DedicacionTexto { get; set; } = string.Empty; // "1,5 DS"
        public int UdbId { get; set; }
        public string? NombreUdb { get; set; }
        public decimal Puntos { get; set; }
        public bool EnLicencia { get; set; }
        public int? ReemplazaACargoId { get; set; }
        public string? ReemplazaADescripcion { get; set; }          // "Perez, Juan – PAd 1 DE"
        public string? Observaciones { get; set; }
        public List<ActividadDto> Actividades { get; set; } = new();
    }
}
