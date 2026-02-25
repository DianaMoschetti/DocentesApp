using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesEFCore.Domain
{
    internal class Cargo
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } // Profesor / Jefe de Trabajos Prácticos / Ayudante de Primera / Ayudante de Segunda

        public float Puntos { get; set; }

        // Navigation properties

        public TipoCargo TipoCargo { get; set; } // 1:1 (un cargo tiene un tipo de cargo)
        public Condicion Condicion { get; set; }
        public Dedicacion? Dedicacion { get; set; }

    }
}
