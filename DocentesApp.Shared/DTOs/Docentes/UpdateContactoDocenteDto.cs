using System.ComponentModel.DataAnnotations;

namespace DocentesApp.Shared.DTOs.Docentes
{
    public class UpdateContactoDocenteDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
    }
}
