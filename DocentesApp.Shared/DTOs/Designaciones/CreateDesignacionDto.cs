using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Designaciones
{
    public class CreateDesignacionDto
    {
        public int DocenteId { get; set; }
        // [Diana desde v4.0 OBSOLETO] CargoId y DedicacionId eliminados — pasan a DetalleDesignacion
        // public int CargoId { get; set; }
        // public int DedicacionId { get; set; }
        // public int? AsignaturaId { get; set; }
        // public int? CursoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        // public decimal? PuntosUtilizados { get; set; }
        // [Diana desde v4.0 OBSOLETO] PuntosLibres reemplazado por el total de puntos de los cargos
        // public decimal? PuntosLibres { get; set; }
        public Estado EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
        // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos (v5). Se comenta cuando se reescriba el form de Blazor.
        [Obsolete("Usar Cargos.")]
        public List<CreateDetalleDesignacionDto> Detalles { get; set; } = new();
        public List<UpsertCargoDesignacionDto> Cargos { get; set; } = new(); // 1..N
    }
}