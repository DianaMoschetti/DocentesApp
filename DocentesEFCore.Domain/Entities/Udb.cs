using DocentesApp.Domain.Base;


namespace DocentesApp.Domain.Entities
{
    public class Udb : BaseDomainModel 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        // Navigation Properties
        public int? DirectorDocenteId { get; set; }
        public Docente? Director { get; set; }
        public int? SecretarioDocenteId { get; set; }
        public Docente? Secretario { get; set; }
        public ICollection<Asignatura> Asignaturas { get; set; } = new HashSet<Asignatura>();
        public ICollection<DocenteUdb> DocenteUdbs { get; set; } = new HashSet<DocenteUdb>();

    }
}
