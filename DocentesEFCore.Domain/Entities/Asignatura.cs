using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;
namespace DocentesApp.Domain.Entities
{
    public class Asignatura : BaseDomainModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!; // nombre libre de la asignatura, no enum
        public bool EsVigente { get; set; } = true; // indica si la asignatura está activa en el plan de estudios
        public Frecuencia Frecuencia { get; set; }
        public Nivel Nivel { get; set; } // Primer año, segundo, tercero
        // Navigation properties
        public int UdbId { get; set; } // 1:n -> Una materia pertenece a una udb y una udb tiene muchas materias FK UDbId + Udb (nav)
        public Udb Udb { get; set; } = null!;
        public ICollection<Actividad> Actividades { get; set; } = new HashSet<Actividad>(); // 1:n -> Una materia está en muchas actividades de cargos
        public ICollection<AsignaturaModulo> AsignaturaModulos { get; set; } = new HashSet<AsignaturaModulo>(); // n:n -> Una materia esta en muchos cursos y un curso tiene muchas materias           
    }
}