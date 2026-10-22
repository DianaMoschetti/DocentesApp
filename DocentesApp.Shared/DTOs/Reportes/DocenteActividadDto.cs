using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Reportes
{
    // Un docente en la vista por curso: una Actividad de un CargoDesignacion vigente.
    public class DocenteActividadDto
    {
        public int CargoDesignacionId { get; set; }
        public int DocenteId { get; set; }
        public string Apellido { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public CargoDocente Cargo { get; set; }
        public string CargoTexto { get; set; } = string.Empty;          // "Profesor Adjunto"
        public Condicion Condicion { get; set; }
        public string CondicionTexto { get; set; } = string.Empty;
        public EspecificacionCargo Especificacion { get; set; }
        public string EspecificacionTexto { get; set; } = string.Empty;
        public string? Horario { get; set; }
        public bool EnLicencia { get; set; }
        public bool EsSuplente { get; set; }                            // Condicion = Suplente
        public string? ReemplazaA { get; set; }                         // "Perez, Juan – PAd 1 DE", si reemplaza a un cargo
    }
}
