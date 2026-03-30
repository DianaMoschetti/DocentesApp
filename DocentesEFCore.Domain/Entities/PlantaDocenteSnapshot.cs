using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class PlantaDocenteSnapshot
    {
        public int Id { get; set; }
        public int PlantaSnapshotId { get; set; }
        public PlantaSnapshot PlantaSnapshot { get; set; }

        // Campos copiados desde la importación (estructura de la planilla)
        public int? DocenteId { get; set; }
        public int? AsignaturaId { get; set; }
        public int? UdbId { get; set; }
        public string? Ano { get; set; }
        public string? Division { get; set; }
        public string? EspecialidadTextoOriginal { get; set; } // desde archivo
        public Especialidad? Especialidad { get; set; }  // desde enum
        public Materia? MateriaEnum { get; set; }          // normalizado, int en BD
        public string? MateriaTextoOriginal { get; set; }   // raw del Excel
        public string? CategoriaDocente { get; set; }
        public string? DedicacionTexto { get; set; }
        public string? Observaciones { get; set; }

        // Opcional: clave natural / hash para detectar duplicates
        public string? RowKey { get; set; }
    }
}
