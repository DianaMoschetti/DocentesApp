using DocentesApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DocentesApp.Shared.DTOs.Docentes
{
    public class CreateDocenteDto
    {
        [Required]
        [MaxLength(15)]
        public string Dni { get; set; } = null!;
        [Range(1, int.MaxValue)]
        public int Legajo { get; set; }
        [Required]        
        public string Nombre { get; set; } = null!;
        [Required]
        public string Apellido { get; set; } = null!;
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public Titulo? MaxNivelAcademico { get; set; } // enum
        public string? Observaciones { get; set; }
    }
}
