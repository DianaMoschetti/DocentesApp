using Domain.Model;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    public class Cargo : BaseDomainModel
    {
        public int Id { get; set; }
        public DenominacionCargo Denominacion { get; set; } // Profesor / Jefe de Trabajos Prácticos / Ayudante de Primera / Ayudante de Segunda / Becario / Administrativo
        public TipoCargo TipoCargo { get; set; } // Adjunto / Asociado / Titular
        public EspecificacionCargo DetalleCargo { get; set; } // Docencia / Gestion / Investigacion
        public Condicion Condicion { get; set; } // Regular / Interino / Suplente / LicenciaConHaberes / LicenciaSinHaberes / Otros
        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; } // Ver si es necesario
        public DateOnly FechaDesignacion { get; set; } // Ver si es necesario
        public DateOnly FechaInicio { get; set; }
        public float Puntos { get; set; } // Ver si es necesario
        public float PuntosUtilizados { get; set; }
        public float PuntosLibres { get; set; }
        public Estado EstadoDesignacion { get; set; } // Activa, Finalizada, Otros
        public string? Observaciones { get; set; }


        // Navigation properties

        public List<Dedicacion>? Dedicacion { get; set; } // Relación uno a uno con Dedicacion
        public List<Asignatura>? Materias { get; set; } // n:n -> Un cargo puede estar asociado a muchas materias y una materia puede tener muchos cargos

    }
}

