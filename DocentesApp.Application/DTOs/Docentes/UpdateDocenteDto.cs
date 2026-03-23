using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class UpdateDocenteDto 
    {
        //lo que mande en null se pisa en la bd
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? MaxNivelAcademico { get; set; }
        public string? Observaciones { get; set; }
    }
}
