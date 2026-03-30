using DocentesApp.Domain.Base;

namespace DocentesApp.Model
{
    public class Laboratorio : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Lugar { get; set; }
        public int? DirectorDocenteId { get; set; }
        public Docente? Director { get; set; }
        public ICollection<Docente> Docentes { get; set; } = new HashSet<Docente>();

        // Docente Responsable
    }
}
