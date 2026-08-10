
namespace DocentesApp.Shared.DTOs.Reportes
{
    public class PlantaDocenteReporteDto
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public int Legajo { get; set; }
        public string Denominacion { get; set; } = string.Empty;
        public string TipoCargo { get; set; } = string.Empty;
        public string Condicion { get; set; } = string.Empty;
        public string TipoDedicacion { get; set; } = string.Empty;
        public float CantidadDedicacion { get; set; }
        public string Especificacion { get; set; } = string.Empty;
        public string? Asignatura { get; set; }
        public decimal PuntosAsignados { get; set; }
        public string? NroResolucion { get; set; }
    }
}
