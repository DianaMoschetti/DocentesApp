
using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class UpdateDesignacionDto // util para corregir datos administrativos (resolucion, nota, observaciones, etc.) 
    {
        // [Diana desde v4.0 OBSOLETO] CargoId y DedicacionId eliminados: el cargo y la dedicación están en cada CargoDesignacion
        //public int CargoId { get; set; }
        //public int DedicacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        // [Diana desde v4.0 OBSOLETO] PuntosLibres reemplazado por el total de puntos de los cargos
        //public decimal? PuntosLibres { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos (v5)
        //public List<CreateDetalleDesignacionDto> Detalles { get; set; } = new();
        // Cargos con Id = existentes (se actualizan), sin Id = nuevos; los que no vienen se eliminan
        public List<UpsertCargoDesignacionDto> Cargos { get; set; } = new();
    }

}
