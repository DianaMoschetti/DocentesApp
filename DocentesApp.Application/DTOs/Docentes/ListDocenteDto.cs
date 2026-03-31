namespace DocentesApp.Application.DTOs.Docentes
{
    public class ListDocenteDto
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
    }
}
