using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class Designacion : BaseDomainModel
    {
        // Designacion (relaciona Docente + Cargo + Asignatura + Dedicacion)
        // Esta entidad captura la designación histórica con fechas y observaciones.
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public Docente Docente { get; set; }
        //public int? AsignaturaId { get; set; } // puede no tener asignatura (ej cargos de gestión)
        //public Asignatura? Asignatura { get; set; }

        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; } // Ver si es necesario
        public DateTime FechaInicio{ get; set; } //  actual = FechaInicio <= hoy && (FechaFin == null || FechaFin > hoy
        public DateTime? FechaFin { get; set; } // historial
        public string? Observaciones { get; set; }
        //public decimal PuntosUtilizados { get; set; }
        public decimal PuntosLibres { get; set; }     // diana ver si sirve de algo, 
        public Estado EstadoDesignacion { get; set; } // Activa, Finalizada
        //public int CursoId { get; set; }
        //public Curso? Curso { get; set; }

        // Navigation Properties
        public ICollection<DetalleDesignacion> Detalles { get; set; } = new HashSet<DetalleDesignacion>();
    }
}