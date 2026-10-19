using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Reportes
{
    // Fila cruda de la vista por curso: una Actividad con asignatura de un CargoDesignacion vigente.
    // Las de docencia traen curso; las de cátedra (Jefe de Cátedra, Laboratorio) no.
    // La devuelve IDocentesPorCursoRepository y la transforma DocentesPorCursoArmador.
    public class DocenteCursoRegistro
    {
        public int CargoDesignacionId { get; set; }
        public int DocenteId { get; set; }
        public string DocenteApellido { get; set; } = string.Empty;
        public string DocenteNombre { get; set; } = string.Empty;

        public CargoDocente Cargo { get; set; }
        public Condicion Condicion { get; set; }
        public bool EnLicencia { get; set; }

        // Cargo que reemplaza (solo suplentes con ReemplazaACargoId)
        public string? ReemplazaApellido { get; set; }
        public string? ReemplazaNombre { get; set; }
        public CargoDocente? ReemplazaCargo { get; set; }
        public decimal? ReemplazaCantidadDedicacion { get; set; }
        public TipoDedicacion? ReemplazaTipoDedicacion { get; set; }

        public EspecificacionCargo Especificacion { get; set; }
        public string? Horario { get; set; }

        public int AsignaturaId { get; set; }
        public string AsignaturaNombre { get; set; } = string.Empty;

        public int? CursoId { get; set; }
        public Nivel? CursoAño { get; set; }
        public Especialidad? CursoCarrera { get; set; }
        public int? CursoNroComision { get; set; }
        public Turno? CursoTurno { get; set; }
    }
}
