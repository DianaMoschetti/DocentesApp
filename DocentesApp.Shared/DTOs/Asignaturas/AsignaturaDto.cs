using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Asignaturas
{
    public class AsignaturaDto
    {
        public int Id { get; set; }
        public int NombreAsignatura { get; set; } // enum 
        public string NombreAsignaturaTexto { get; set; } = string.Empty;
        public int Frecuencia { get; set; } // enum
        public string FrecuenciaTexto { get; set; } = string.Empty;
        public int Nivel { get; set; } // enum Primer año, segundo, tercero
        public string NivelTexto { get; set; } = string.Empty;
        public int? UdbId { get; set; }
        public string? NombreUdb { get; set; }
    }
}
