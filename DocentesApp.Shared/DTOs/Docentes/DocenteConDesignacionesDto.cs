using DocentesApp.Shared.DTOs.Designaciones;

namespace DocentesApp.Shared.DTOs.Docentes
{
    public class DocenteConDesignacionesDto
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] el legajo pasa a ser opcional (v5)
        //public int Legajo { get; set; }
        public int? Legajo { get; set; }
        public string NombreCompleto { get; set; }       
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
        public List<ListDesignacionDto> Designaciones { get; set; } = new(); //List<ListDesignacionDto>();
    }
}
