using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Reportes
{
    // Fila cruda de la consulta de planta por UDB (una por Actividad de un CargoDesignacion vigente de la UDB).
    // La devuelve IPlantaReporteRepository y la transforma PlantaUdbArmador.
    public class PlantaUdbRegistro
    {
        public int CargoDesignacionId { get; set; }
        public string DocenteApellido { get; set; } = string.Empty;
        public string DocenteNombre { get; set; } = string.Empty;
        public int? Legajo { get; set; }

        public CargoDocente Cargo { get; set; }
        public Condicion Condicion { get; set; }
        public TipoDedicacion TipoDedicacion { get; set; }
        public decimal CantidadDedicacion { get; set; }
        public decimal Puntos { get; set; }
        public bool EnLicencia { get; set; }
        public string? CargoObservaciones { get; set; }

        public EspecificacionCargo Especificacion { get; set; }
        public string? Asignatura { get; set; }
        public Nivel? AsignaturaNivel { get; set; }
        public Frecuencia? AsignaturaFrecuencia { get; set; }
        public Nivel? CursoAño { get; set; }
        public Especialidad? CursoCarrera { get; set; }
        public int? CursoNroComision { get; set; }
        public Turno? CursoTurno { get; set; }
        public string? Horario { get; set; }
        public string? ActividadObservaciones { get; set; }
    }
}
