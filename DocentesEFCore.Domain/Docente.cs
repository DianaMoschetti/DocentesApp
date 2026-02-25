using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Docente : Persona
    {
        public int Legajo { get; set; }
        public string Titulo { get; set; } // terciario / universitario / posgrado
        public string MaxNivelAcademico { get; set; } = string.Empty;// especialista / maestro / doctor / investigador
        public string Observaciones { get; set; } = string.Empty;

        // Navigation properties

        //cargos
    }
}
