namespace DocentesApp.Shared.DTOs.Dedicaciones
{
    // [Diana desde v4.0 OBSOLETO] DTO de Dedicacion reemplazado por atributos en CreateDetalleDesignacionDto/DetalleDesignacionDto.
    // Mantener hasta completar el refactor completo.
    [Obsolete("DTO de Dedicacion obsoleto desde v4.0. Los atributos de dedicación pasan a DetalleDesignacion.")]
    public class ListDedicacionDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public float CantidadHoras { get; set; }
        public float CantidadDedicacion { get; set; }
    }
}