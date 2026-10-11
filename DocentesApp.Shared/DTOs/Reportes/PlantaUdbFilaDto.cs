namespace DocentesApp.Shared.DTOs.Reportes
{
    // Una fila del reporte "Planta docente por UDB" = una Actividad de un CargoDesignacion vigente.
    // Las dos vistas (por asignatura / por docente) usan la misma fila con distinto orden y agrupación.
    public class PlantaUdbFilaDto
    {
        // Vista por asignatura: "Gestión", "Laboratorios e Investigación", el nombre de la asignatura u "Otros"
        public string Seccion { get; set; } = string.Empty;
        public string? Asignatura { get; set; }
        public string? AC { get; set; }             // A (anual) / C (cuatrimestral), desde Asignatura.Frecuencia
        public string? Curso { get; set; }          // formato actual del Curso
        public string? Carrera { get; set; }        // desde Curso.Carrera
        public string Docente { get; set; } = string.Empty;
        public int? Legajo { get; set; }
        public string CargoCorto { get; set; } = string.Empty;      // PAd, PAs, PT, JTP, Ay 1º, Ay 2º
        public string CondicionCorta { get; set; } = string.Empty;  // Ord, R, Int, Sup, Otro
        public string Dedicacion { get; set; } = string.Empty;      // "1 DS", "1 DTP", "1,5 DS"
        public string Especificacion { get; set; } = string.Empty;
        public string? Horario { get; set; }
        public string? Observaciones { get; set; }   // de la actividad y/o del cargo (ej. motivo de la licencia)
        public bool EnLicencia { get; set; }
        public int CargoDesignacionId { get; set; }
        // Vista por docente: los puntos del cargo solo en la primera fila de cada cargo, 0 en las demás
        public decimal Puntos { get; set; }
    }
}
