using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class DesignacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompletoDocente { get; set; } = string.Empty;
        // [Diana desde v4.0 OBSOLETO] CargoId y DedicacionId eliminados — pasan a DetalleDesignacion
        // public int CargoId { get; set; }
        // public string DescripcionCargo { get; set; } = string.Empty;
        // public int DedicacionId { get; set; }
        // public string DescripcionDedicacion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        // public decimal? PuntosUtilizados { get; set; }
        public decimal? PuntosLibres { get; set; }
        // public int? AsignaturaId { get; set; }
        // public string? NombreAsignatura { get; set; }
        // public int? CursoId { get; set; }
        // public string? DescripcionCurso { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        public List<DetalleDesignacionDto> Detalles { get; set; } = new();
        public bool EsVigente => FechaFin == null || FechaFin > DateTime.Now;
    }
}