using DocentesEFCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    internal class Comision
    {
        public int Id { get; set; }

        public int Division { get; set; } //13
        public int Año { get; set; } // 1
        public string Carrera { get; set; } // k
        public string Denominacion  { get; set; } // 1k13

        // Navigation properties

        public List<Materia> Materias { get; set; } = new List<Materia>() { }; // 1:n (una comisión puede tener varias materias)
        public Turno Turno { get; set; } = new Turno() { }; // 1:1

        // Carrera (dentro de la denominacion se incluye la carrera)
    }
}
