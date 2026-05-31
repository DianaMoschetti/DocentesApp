using DocentesApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace DocentesApp.Shared.DTOs.Docentes
{
    public class DocenteDto
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public DateOnly? FechaNacimiento { get; set; }
        public string NombreCompleto { get; set; } = null!;
        [MaxLength(15)]
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternativo { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public Titulo? MaxNivelAcademico { get; set; } // enum Titulo
        public string? Observaciones { get; set; }

    }
}
