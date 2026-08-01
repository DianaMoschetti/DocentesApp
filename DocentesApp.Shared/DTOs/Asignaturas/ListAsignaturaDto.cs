using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Asignaturas
{
    public class ListAsignaturaDto
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] NombreAsignaturaTexto reemplazado por Nombre directo
        // public string NombreAsignaturaTexto { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool EsVigente { get; set; }
        // public int Frecuencia { get; set; } // enum
        public string FrecuenciaTexto { get; set; } = string.Empty;
        // public int Nivel { get; set; } // enum Primer año, segundo, tercero
        public string NivelTexto { get; set; } = string.Empty;
        public int? UdbId { get; set; }
        public string? NombreUdb { get; set; }
    }
}