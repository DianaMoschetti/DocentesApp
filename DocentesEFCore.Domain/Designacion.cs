using Domain.Model;
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
        public string NroNota { get; set; } //Ver si es necesario
        public DateOnly FechaDesignacion { get; set; } // ver si es necesario
        public DateOnly FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public string? Observaciones { get; set; }

        // Navigation Properties

        // Docente
        // Cargo
        // Dedicacion   
        // Materia

        // Estado???
    }
}
