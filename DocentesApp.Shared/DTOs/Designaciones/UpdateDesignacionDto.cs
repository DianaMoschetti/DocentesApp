
using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class UpdateDesignacionDto // util para corregir datos administrativos (resolucion, nota, observaciones, etc.) 
    {
        public int CargoId { get; set; }
        public int DedicacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public decimal? PuntosLibres { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        public List<CreateDetalleDesignacionDto> Detalles { get; set; } = new();
    }

}
