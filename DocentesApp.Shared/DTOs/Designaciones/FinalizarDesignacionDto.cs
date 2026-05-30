using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Designaciones
{
    public class FinalizarDesignacionDto
    {
        
        //public int DedicacionId { get; set; }
        
        public DateTime? FechaFin { get; set; }

        public string? NroResolucion { get; set; }
        public string? NroNota { get; set; }
        public decimal? PuntosUtilizados { get; set; }
        public decimal? PuntosLibres { get; set; }
        //public int EstadoDesignacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
