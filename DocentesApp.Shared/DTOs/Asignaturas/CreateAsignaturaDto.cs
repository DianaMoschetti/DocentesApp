using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Asignaturas
{
    public class CreateAsignaturaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public bool EsVigente { get; set; } = true;
        public Frecuencia Frecuencia { get; set; } // enum
        public Nivel Nivel { get; set; } // enum Primer año, segundo, tercero
        public int? UdbId { get; set; }
    }
}