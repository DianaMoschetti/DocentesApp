using Domain.Model;

namespace DocentesEFCore.Domain
{
    public class Persona : BaseDomainModel
    {
        public int Id { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }

    }
}
