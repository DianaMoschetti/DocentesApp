using DocentesApp.Domain.Enums;
namespace DocentesApp.Shared.DTOs.Asignaturas
{
    public class CreateAsignaturaDto
    {
        // [Diana desde v4.0 OBSOLETO] Id no va en el DTO de creación
        // public int Id { get; set; }
        // [Diana desde v4.0 OBSOLETO] NombreAsignatura (enum Materia) reemplazado por Nombre (string)
        // public int NombreAsignatura { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool EsVigente { get; set; } = true;
        public Frecuencia Frecuencia { get; set; } // enum
        public Nivel Nivel { get; set; } // enum Primer año, segundo, tercero
        public int? UdbId { get; set; }
    }
}