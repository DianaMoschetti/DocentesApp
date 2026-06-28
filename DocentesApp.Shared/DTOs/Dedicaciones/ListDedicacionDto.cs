namespace DocentesApp.Shared.DTOs.Dedicaciones
{
    public class ListDedicacionDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; }
    }
}