using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Shared.DTOs.Snapshots
{
    public class PlantaSnapshotDto
    {
        public int Id { get; set; }
        public DateTime SnapshotDate { get; set; }
        public string Fuente { get; set; } = string.Empty; // Ej: "Importación Excel", "API externa", etc.
        public bool IsCurrent { get; set; }
        public List<CreatePlantaSnapshotDto> Filas { get; set; } = new();
    }
}
