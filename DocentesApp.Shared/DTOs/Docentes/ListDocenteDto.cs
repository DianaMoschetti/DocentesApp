namespace DocentesApp.Shared.DTOs.Docentes
{
    public class ListDocenteDto
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] el legajo pasa a ser opcional (v5)
        //public int Legajo { get; set; }
        public int? Legajo { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Dni { get; set; }
        public string? Email { get; set; }
        public string? Observaciones { get; set; }
    }
}
