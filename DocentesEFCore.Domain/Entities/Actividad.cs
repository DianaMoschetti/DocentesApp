using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    // Actividad de un cargo (v5): reemplaza los campos de actividad de DetalleDesignacion.
    // La dedicación NO se reparte entre actividades: solo se listan.
    // Qué datos lleva según la Especificacion: ver Domain.Services.ActividadReglas.
    public class Actividad : BaseDomainModel
    {
        public int Id { get; set; }
        public int CargoDesignacionId { get; set; }
        public CargoDesignacion CargoDesignacion { get; set; } = null!;

        public EspecificacionCargo Especificacion { get; set; }
        public int? AsignaturaId { get; set; } // null = sin asignatura (nunca un registro comodín)
        public Asignatura? Asignatura { get; set; }
        public int? CursoId { get; set; }
        public Curso? Curso { get; set; }

        public string? Horario { get; set; } // texto libre, ej: "TM / MI 4º-5º-6º // LU 5º-6º 10:30 a 12:50"
        public string? Observaciones { get; set; }
    }
}
