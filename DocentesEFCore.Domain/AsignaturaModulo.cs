using DocentesApp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Model
{
    public class AsignaturaModulo
    {
        // en que módulo se dicta la asignatura
        public int AsignaturaId { get; set; }
        public Asignatura Asignatura { get; set; }
        public Modulo Modulo { get; set; }
        public int CursoId { get; set; } // opción para saber en cuál comisión
        public Curso Curso { get; set; }
    }
}
