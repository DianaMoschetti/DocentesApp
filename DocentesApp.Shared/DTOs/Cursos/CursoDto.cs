using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Cursos
{
    public class CursoDto
    {
        public int Id { get; set; }
        public int Turno { get; set; } // enum
        public string TurnoTexto { get; set; } = string.Empty;
        public int Año { get; set; }
        public int Carrera { get; set; } // enum
        public string CarreraTexto { get; set; } = string.Empty;
        public int NroComision { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
