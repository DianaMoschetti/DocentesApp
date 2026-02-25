using Domain.Model;

namespace DocentesEFCore.Domain
{
    public class Laboratorio : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        // Docente Responsable
    }
}
