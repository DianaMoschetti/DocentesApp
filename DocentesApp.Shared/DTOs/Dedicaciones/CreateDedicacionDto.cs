using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Dedicaciones
{
    public class CreateDedicacionDto
    {
        public TipoDedicacion DescTipo { get; set; }
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; }
    }
}