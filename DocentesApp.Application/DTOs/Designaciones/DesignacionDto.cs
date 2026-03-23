using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Designaciones
{
    public class DesignacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string NombreCompletoDocente { get; set; } = string.Empty;
        public int CargoId { get; set; }
        public string DescripcionCargo { get; set; } = string.Empty;
        public int DedicacionId { get; set; }
        public string DescripcionDedicacion { get; set; } = string.Empty;
        public int? AsignaturaId { get; set; }
        public string? NombreAsignatura { get; set; }
        public int? CursoId { get; set; }
        public string? DescripcionCurso { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public decimal? PuntosUtilizados { get; set; }
        public decimal? PuntosLibres { get; set; }
        public int EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
