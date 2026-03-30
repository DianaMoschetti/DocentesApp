
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
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
