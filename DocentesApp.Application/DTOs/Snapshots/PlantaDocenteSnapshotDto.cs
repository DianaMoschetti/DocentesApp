using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.DTOs.Snapshots
{
    public class PlantaDocenteSnapshotDto
    {
        public int Id { get; set; }
        public int PlantaSnapshotId { get; set; }
        public int? DocenteId { get; set; }
        public string? DocenteNombreCompleto { get; set; }

        public int? AsignaturaId { get; set; }
        public string? AsignaturaNombre { get; set; }

        public int? UdbId { get; set; }
        public string? UdbNombre { get; set; }

        public string? Ano { get; set; }
        public string? Division { get; set; }
        public string? EspecialidadTextoOriginal { get; set; }
        public string? MateriaTextoOriginal { get; set; }
        public string? CategoriaDocente { get; set; }
        public string? DedicacionTexto { get; set; }
        public string? Observaciones { get; set; }
        public string RowKey { get; set; } = string.Empty;
    }
}
