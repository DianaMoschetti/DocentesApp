using DocentesApp.Model;
using DocentesApp.Model.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class Docente : Persona
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public Titulo Titulo { get; set; } // terciario / universitario / posgrado
        public string MaxNivelAcademico { get; set; } = string.Empty;// especialista / maestro / doctor / investigador
        public string Observaciones { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Designacion> Designaciones { get; set; } = new HashSet<Designacion>(); // 1:n -> Un docente puede tener uno o más cargos a lo largo de su carrera.
        public ICollection<DocenteUdb> DocenteUdbs { get; set; } = new HashSet<DocenteUdb>(); // 1:n -> Un docente puede pertenecer a una o más udbs.
    }
}
