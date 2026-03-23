using DocentesApp.Model.Enums;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Asignaturas
{
    public class CreateAsignaturaDto
    {
        public int Id { get; set; }
        public int NombreAsignatura { get; set; } // enum 
        public int Frecuencia { get; set; } // enum
        public int Nivel { get; set; } // enum Primer año, segundo, tercero             
        public int? UdbId { get; set; }
}
