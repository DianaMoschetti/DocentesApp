using Domain.Model;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Docente : Persona
    {
        public int Legajo { get; set; }
        public Titulo Titulo { get; set; } // terciario / universitario / posgrado
        public string MaxNivelAcademico { get; set; } = string.Empty;// especialista / maestro / doctor / investigador
        public string Observaciones { get; set; } = string.Empty;

        //participa en proy de investigacion?

        // Navigation properties

        public List<Asignatura>? Materias { get; set; } // n:n -> Un docente da en muchas materias y una materia tiene muchos docentes
        public List<Cargo>? Cargos { get; set; } // 1:n -> Un docente puede tener uno o más cargos a lo largo de su carrera.
        public List<UDB> Udb { get; set; } // 1:n -> Un docente puede pertenecer a una o más udbs.
    }
}
