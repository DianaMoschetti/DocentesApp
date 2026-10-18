using DocentesApp.Domain.Base;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    // [Diana desde v4.0 OBSOLETO] ahora hereda de BaseDomainModel (auditoría)
    //public class Curso //Comision
    public class Curso : BaseDomainModel //Comision
    {
        // Curso/Comision y Modulos (estructura de horarios)

        public int Id { get; set; }
        public Turno Turno { get; set; }
        public int NroComision { get; set; } //13
        public Nivel Año { get; set; } // 1  // 1 / 2 / 3 / 4 / 5
        public Especialidad Carrera { get; set; } // k - isi
        //public string Denominacion { get; set; } // 1k13 se puede armar sin necesidad de persistirla

        // Navigation properties
        public ICollection<AsignaturaModulo> AsignaturaModulos { get; set; } = new HashSet<AsignaturaModulo>(); // 1:n (una comisión puede tener varias materias)

        //public List<Docente> Docentes { get; set; } //n:n (una comisión puede tener varios docentes y un docente puede dar en varias comisiones)
    }
}
