using DocentesApp.Model.Enums;
using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Snapshots
{
    public class CreatePlantaSnapshotDto
    {
        public string Fuente { get; set; } = string.Empty; // Ej: "Importación Excel", "API externa", etc.
        public bool IsCurrent { get; set; }

        public List<CreatePlantaDocenteSnapshotDto> Filas { get; set; } = new();
       
    }
}
