using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class ListDesignacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompletoDocente { get; set; } = string.Empty;
        // [Diana desde v4.0 OBSOLETO] DescripcionCargo y DescripcionDedicacion pasan a DetalleDesignacion
        // public string DescripcionCargo { get; set; } = string.Empty;
        // public int DedicacionId { get; set; }
        // public string DescripcionDedicacion { get; set; } = string.Empty;
        // public int? AsignaturaId { get; set; }
        // public string? NombreAsignatura { get; set; }
        // public int? CursoId { get; set; }
        // public string? DescripcionCurso { get; set; }
        public string? NroResolucion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public Estado EstadoDesignacion { get; set; }
        // [Diana desde v4.0 OBSOLETO] vigente = Activa y sin FechaFin o con FechaFin futura
        //public bool EsVigente => FechaFin == null || FechaFin > DateTime.Now;
        public bool EsVigente => EstadoDesignacion == Estado.Activa && (FechaFin == null || FechaFin > DateTime.Now);
        public decimal TotalPuntos { get; set; } // suma de los puntos de los cargos
        // public string? NroResolucion { get; set; }
        // public string? NroNota { get; set; }
        // public decimal? PuntosUtilizados { get; set; }
        // public decimal? PuntosLibres { get; set; }
        // public int EstadoDesignacion { get; set; }
        // public string? Observaciones { get; set; }
    }
}