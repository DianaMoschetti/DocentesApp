using DocentesApp.Model;

namespace DocentesApp.Model
{
    public class Laboratorio : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Lugar { get; set; }
        public int IdDirector { get; set; }
        public ICollection<Docente> Docentes { get; set; } = new HashSet<Docente>();

        // Docente Responsable
    }
}
