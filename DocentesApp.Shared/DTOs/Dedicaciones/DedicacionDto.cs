using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Dedicaciones
{
    public class DedicacionDto
    {
        public int Id { get; set; }
        public TipoDedicacion DescTipo { get; set; }
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}