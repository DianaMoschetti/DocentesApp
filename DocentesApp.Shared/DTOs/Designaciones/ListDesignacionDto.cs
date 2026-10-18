using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class ListDesignacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompletoDocente { get; set; } = string.Empty;
        public string? NroResolucion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public Estado EstadoDesignacion { get; set; }
        // vigente = Activa y sin FechaFin o con FechaFin futura
        public bool EsVigente => EstadoDesignacion == Estado.Activa && (FechaFin == null || FechaFin > DateTime.Now);
        public decimal TotalPuntos { get; set; } // suma de los puntos de los cargos
    }
}