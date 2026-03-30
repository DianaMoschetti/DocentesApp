using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class PlantaSnapshot
    {
        public int Id { get; set; }
        public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;
        public string Fuente { get; set; } // "Import Excel Quimica 2026-03-01" o "Ajuste manual"
        public bool IsCurrent { get; set; } = false;

        // Navigation properties
        public ICollection<PlantaDocenteSnapshot> Filas { get; set; } = new HashSet<PlantaDocenteSnapshot>();
    }
}
