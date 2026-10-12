using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    // Actividad de un cargo, para alta y edición. Id null = actividad nueva.
    // Qué datos lleva según la Especificacion: ver Domain.Services.ActividadReglas.
    public class UpsertActividadDto
    {
        public int? Id { get; set; }
        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; } // null = sin asignatura
        public int? CursoId { get; set; }
        public string? Horario { get; set; } // texto libre
        public string? Observaciones { get; set; }
    }
}
