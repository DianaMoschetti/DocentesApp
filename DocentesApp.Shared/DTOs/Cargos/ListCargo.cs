namespace DocentesApp.Shared.DTOs.Cargos
{
    public class ListCargoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty; // "Titular Adjunto Docencia Ordinario"
        public float PuntosBase { get; set; }
    }
}