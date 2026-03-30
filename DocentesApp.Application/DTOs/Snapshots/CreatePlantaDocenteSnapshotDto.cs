using DocentesApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.DTOs.Snapshots
{
    public class CreatePlantaDocenteSnapshotDto
    {
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

        public string RowKey { get; set; } = string.Empty;
    } 
}
