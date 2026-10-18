
using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class UpdateDesignacionDto // util para corregir datos administrativos (resolucion, nota, observaciones, etc.) 
    {
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        // Cargos con Id = existentes (se actualizan), sin Id = nuevos; los que no vienen se eliminan
        public List<UpsertCargoDesignacionDto> Cargos { get; set; } = new();
    }

}
