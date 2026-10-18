using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class DesignacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompletoDocente { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public decimal TotalPuntos { get; set; } // suma de los puntos de los cargos
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        public List<CargoDesignacionDto> Cargos { get; set; } = new();
        // vigente = Activa y sin FechaFin o con FechaFin futura
        public bool EsVigente => EstadoDesignacion == Estado.Activa && (FechaFin == null || FechaFin > DateTime.Now);
    }
}