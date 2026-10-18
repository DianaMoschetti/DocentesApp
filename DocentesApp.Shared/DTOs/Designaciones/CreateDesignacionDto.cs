using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class CreateDesignacionDto
    {
        public int DocenteId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        public List<UpsertCargoDesignacionDto> Cargos { get; set; } = new(); // 1..N
    }
}