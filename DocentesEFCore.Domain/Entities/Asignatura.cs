using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class Asignatura
    {
        public int Id { get; set; }
        public Materia NombreAsignatura { get; set; }
        public Frecuencia Frecuencia { get; set; }
        public Nivel Nivel { get; set; } // Primer año, segundo, tercero

        // Navigation properties
        public int? UdbId { get; set; } // 1:n -> Una materia pertenece a una udb y una udb tiene muchas materias FK UDbId + Udb (nav)
        public Udb? Udb { get; set; } // 

        public ICollection<Designacion> Designaciones { get; set; } = new HashSet<Designacion>(); // 1:n -> Una materia tiene muchos docentes y un docente da en una o muchas materias
        public ICollection<AsignaturaModulo> AsignaturaModulos { get; set; } = new HashSet<AsignaturaModulo>(); // n:n -> Una materia esta en muchos cursos y un curso tiene muchas materias           

    }
}
