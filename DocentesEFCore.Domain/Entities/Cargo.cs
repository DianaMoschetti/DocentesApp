using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class Cargo : BaseDomainModel
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; } // Profesor / Jefe de Trabajos Prácticos / Ayudante de Primera / Ayudante de Segunda / Becario / Administrativo
        public TipoCargo TipoCargo { get; set; } // Adjunto / Asociado / Titular
        public EspecificacionCargo DetalleCargo { get; set; } // Docencia / Gestion / Investigacion
        public Condicion Condicion { get; set; } // Regular / Interino / Suplente / LicenciaConHaberes / LicenciaSinHaberes / Otros
        public float PuntosBase { get; set; }
        public string? Observaciones { get; set; }


        // Navigation properties
        public ICollection<Designacion> Designaciones { get; set; } = new HashSet<Designacion>(); //se puede borrar si es lio

    }
}

