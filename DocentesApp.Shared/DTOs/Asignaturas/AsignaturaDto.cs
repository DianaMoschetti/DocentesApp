using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Asignaturas
{
    public class AsignaturaDto
    {
        public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] NombreAsignatura (enum) reemplazado por Nombre (string)
        // public int NombreAsignatura { get; set; } // enum
        // public string NombreAsignaturaTexto { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool EsVigente { get; set; }
        public Frecuencia Frecuencia { get; set; } // enum
        public string FrecuenciaTexto { get; set; } = string.Empty;
        public Nivel Nivel { get; set; } // enum Primer año, segundo, tercero
        public string NivelTexto { get; set; } = string.Empty;
        public int? UdbId { get; set; }
        public string? NombreUdb { get; set; }
    }
}