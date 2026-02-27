using Domain.Model;

namespace DocentesEFCore.Domain
{
    public class Laboratorio : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Lugar { get; set; }
        public int IdDirector { get; set; }
        public List<Docente> Docentes { get; set; }

        // Docente Responsable
    }
}
