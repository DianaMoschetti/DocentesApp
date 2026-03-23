using DocentesApp.Application.DTOs.Designaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class DetalleDocenteDto
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public string NombreCompleto { get; set; }       
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
        public List<ListDesignacionDto> Designaciones { get; set; } = new(); //List<ListDesignacionDto>();
    }
}
