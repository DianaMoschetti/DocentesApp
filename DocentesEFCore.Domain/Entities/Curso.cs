using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class Curso //Comision
    {
        // Curso/Comision y Modulos (estructura de horarios)

        public int Id { get; set; }
        public Turno Turno { get; set; }
        public int NroComision { get; set; } //13
        public Nivel Año { get; set; } // 1  // 1 / 2 / 3 / 4 / 5
        public Especialidad Carrera { get; set; } // k - isi
        //public string Denominacion { get; set; } // 1k13 se puede armar sin necesidad de persistirla

        // Navigation properties
        public ICollection<AsignaturaModulo> AsignaturaModulos { get; set; } = new HashSet<AsignaturaModulo>(); // 1:n (una comisión puede tener varias materias)

        //public List<Docente> Docentes { get; set; } //n:n (una comisión puede tener varios docentes y un docente puede dar en varias comisiones)
    }
}
