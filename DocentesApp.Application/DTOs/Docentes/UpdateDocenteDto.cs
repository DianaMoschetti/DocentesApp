using System.ComponentModel.DataAnnotations;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class UpdateDocenteDto 
    {
        [Required]
        public string Nombre { get; set; } = null!;
        [Required]
        public string Apellido { get; set; } = null!;
        [MaxLength(15)]
        public string? Dni { get; set; }
       
        [Range(1, int.MaxValue)]
        public int Legajo { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? FechaNacimiento { get; set; }
        public string? MaxNivelAcademico { get; set; }
        public string? Observaciones { get; set; }
    }
}
