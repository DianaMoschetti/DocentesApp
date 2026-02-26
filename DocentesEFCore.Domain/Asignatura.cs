using DocentesEFCore.Domain;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    internal class Asignatura
    {
        public int Id { get; set; }
        public Materia NombreAsignatura { get; set; }
        public Frecuencia Frecuencia { get; set; }
        public int Nivel { get; set; } // Primer año, segundo, tercero

        // Navigation properties
        public List<Docente> Docentes { get; set; } // 1:n -> Una materia tiene muchos docentes y un docente da en una o muchas materias
        public int IdUdb { get; set; } // 1:n -> Una materia pertenece a una udb y una udb tiene muchas materias
        public List<Curso> Comisiones { get; set; } // n:n -> Una materia esta en muchos cursos y un curso tiene muchas materias           
        

    }
}
