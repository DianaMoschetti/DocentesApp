using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class DocenteDto
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public string NombreCompleto { get; set; } = null!;
        [MaxLength(15)]
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? MaxNivelAcademico { get; set; }
        public string? Observaciones { get; set; }

    }
}
