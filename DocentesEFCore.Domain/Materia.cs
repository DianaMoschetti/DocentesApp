using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public int Nivel { get; set; } // Primer año, segundo, tercero

        public bool EsCuatrimestral { get; set; }
        public bool EsElectiva { get; set; }

        // Navigation properties

        public Comision Comision { get; set; }

        // Docentes
    }
}
