using DocentesApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace DocentesApp.Shared.DTOs.Docentes
{
    public class UpdateDocenteDto 
    {
        [Required]
        public string Nombre { get; set; } = null!;
        [Required]
        public string Apellido { get; set; } = null!;
        [MaxLength(15)]
        public string? Dni { get; set; }
       
        // [Diana desde v4.0 OBSOLETO] Range(1, 8) rechazaba casi cualquier legajo real
        //[Range(1, 8)]
        // [Diana desde v4.0 OBSOLETO] el legajo pasa a ser opcional (v5)
        //public int Legajo { get; set; }
        [Range(1, int.MaxValue)] // opcional; si viene, mayor a 0 (Range ignora null), igual que UpdateDocenteDtoValidator
        public int? Legajo { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? FechaNacimiento { get; set; }
        public Titulo? MaxNivelAcademico { get; set; } // enum
        public string? Observaciones { get; set; } = string.Empty;
    }
}
