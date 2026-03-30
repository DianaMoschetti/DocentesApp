using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    public class Dedicacion 
    {
        public int Id { get; set; }
        public TipoDedicacion DescTipo { get; set; } // Simples (10hs) Exclusiva (40)        
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; } //una dedicacion, media dedicacion, una y media
    }
}
