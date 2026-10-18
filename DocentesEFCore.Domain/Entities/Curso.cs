using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class Curso : BaseDomainModel //Comision
    {
        // Curso/Comision y Modulos (estructura de horarios)

        public int Id { get; set; }
        public Turno Turno { get; set; }
        public int NroComision { get; set; } //13
        public Nivel Año { get; set; } // 1  // 1 / 2 / 3 / 4 / 5
        public Especialidad Carrera { get; set; } // k - isi

        // Navigation properties
        public ICollection<AsignaturaModulo> AsignaturaModulos { get; set; } = new HashSet<AsignaturaModulo>(); // 1:n (una comisión puede tener varias materias)
    }
}
