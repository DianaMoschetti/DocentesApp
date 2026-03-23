using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Cursos
{
    public class UpdateCursoDto
    {
        public int Turno { get; set; } // enum
        public int Año { get; set; }
        public int Carrera { get; set; } // enum
        public int NroComision { get; set; }
    }
}
