using DocentesApp.Model;

namespace DocentesApp.Model
{
    public class Persona : BaseDomainModel
    {
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }

    }
}
