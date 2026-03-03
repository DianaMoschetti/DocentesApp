using DocentesApp.Model.Enums;

namespace DocentesApp.Model
{
    public class DocenteUdb
    {
        // Tabla intermedia para Docente <-> UDB (un docente puede pertenecer a varias UDB)
        public int DocenteId { get; set; }
        public Docente Docente { get; set; }
        public int UdbId { get; set; }
        public Udb Udb { get; set; }
        public RolUdb Rol { get; set; } // "Director", "Secretario", "Becario", "Docente"
    }
}
