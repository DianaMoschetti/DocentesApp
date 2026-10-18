using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    // Un cargo dentro de una Designacion (v5). Los puntos pertenecen al cargo y se cuentan una sola vez,
    // aunque el cargo tenga varias actividades (asignaturas/cursos).
    // [Diana desde v4.0 OBSOLETO] ahora hereda de BaseDomainModel (auditoría)
    //public class CargoDesignacion
    public class CargoDesignacion : BaseDomainModel
    {
        public int Id { get; set; }
        public int DesignacionId { get; set; }
        public Designacion Designacion { get; set; } = null!;

        public CargoDocente Cargo { get; set; }
        public Condicion Condicion { get; set; } // la Condicion no influye en los puntos
        public TipoDedicacion TipoDedicacion { get; set; }
        public decimal CantidadDedicacion { get; set; } // 0,5 / 1 / 1,5 / 2 ...

        public int UdbId { get; set; } // obligatoria: todas las asignaturas de sus actividades deben ser de esta UDB
        public Udb Udb { get; set; } = null!;

        // Calculado por el backend (CalculadoraPuntos) y guardado. Solo se recalcula cuando cambia el propio cargo
        // (Cargo, dedicación, cantidad, EnLicencia): cambios posteriores en PuntosPorCargo no alteran designaciones existentes.
        public decimal Puntos { get; set; }

        // Un cargo en licencia sigue en la planta, con 0 puntos; Observaciones explica el motivo.
        public bool EnLicencia { get; set; }

        // Suplencia: el cargo del suplente apunta al cargo en licencia que reemplaza.
        public int? ReemplazaACargoId { get; set; }
        public CargoDesignacion? ReemplazaACargo { get; set; }
        public ICollection<CargoDesignacion> Reemplazos { get; set; } = new HashSet<CargoDesignacion>();

        public string? Observaciones { get; set; }

        public ICollection<Actividad> Actividades { get; set; } = new HashSet<Actividad>(); // 1..N
    }
}
