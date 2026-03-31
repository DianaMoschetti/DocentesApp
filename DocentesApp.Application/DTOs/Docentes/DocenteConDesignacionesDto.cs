using DocentesApp.Application.DTOs.Designaciones;

namespace DocentesApp.Application.DTOs.Docentes
{
    public class DocenteConDesignacionesDto
    {
        public int Id { get; set; }
        public int Legajo { get; set; }
        public string NombreCompleto { get; set; }       
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
        public List<ListDesignacionDto> Designaciones { get; set; } = new(); //List<ListDesignacionDto>();
    }
}
