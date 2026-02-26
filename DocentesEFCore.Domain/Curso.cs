using DocentesEFCore.Domain;
using Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    internal class Curso //Comision
    {
        public int Id { get; set; }
        public Turno Turno { get; set; } = new Turno() { };
        public int NroComision { get; set; } //13
        public Nivel Año { get; set; } // 1  // 1 / 2 / 3 / 4 / 5
        public Especialidad Carrera { get; set; } // k - isi
        public string Denominacion { get; set; } // 1k13
        public List<Modulo> Modulos { get; set; }

        // Navigation properties

        public List<Asignatura> Materias { get; set; } = new List<Asignatura>() { }; // 1:n (una comisión puede tener varias materias)
       
        public List<Docente> Docentes { get; set; } //n:n (una comisión puede tener varios docentes y un docente puede dar en varias comisiones)
    }
}
