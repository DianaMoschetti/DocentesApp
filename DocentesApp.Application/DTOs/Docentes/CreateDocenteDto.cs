using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class CreateDocenteDto
    {
        [Required]
        [MaxLength(10)]
        public string Dni { get; set; }
        [Required]
        public int Legajo { get; set; }
        [Required]        
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public string? MaxNivelAcademico { get; set; }
        public string? Observaciones { get; set; }
    }
}
