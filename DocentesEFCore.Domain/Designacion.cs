using Domain.Model;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Designacion : BaseDomainModel
    {
        public int Id { get; set; }
        public string NroResolucion { get; set; }
        public string NroNota { get; set; } // Ver si es necesario
        public DateOnly FechaDesignacion { get; set; } // Ver si es necesario
        public DateOnly FechaInicio { get; set; }
        public string? Observaciones { get; set; }
        public float PuntosUtilizados { get; set; }
        public float PuntosLibres { get; set; }
        public Estado EstadoDesignacion { get; set; } // Activa, Finalizada

        // Navigation Properties
        public Docente Docente { get; set; } // Relacion 1:1 con docente o puede tener mas de un docente???

        // Cargo
        // Dedicacion   
        // Materia
    }
}