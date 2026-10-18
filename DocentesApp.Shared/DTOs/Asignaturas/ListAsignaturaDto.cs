using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Asignaturas
{
    public class ListAsignaturaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsVigente { get; set; }
        public string FrecuenciaTexto { get; set; } = string.Empty;
        public string NivelTexto { get; set; } = string.Empty;
        public int UdbId { get; set; }
        public string? NombreUdb { get; set; }
    }
}