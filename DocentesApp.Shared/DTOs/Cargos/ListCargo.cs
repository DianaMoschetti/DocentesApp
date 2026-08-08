namespace DocentesApp.Shared.DTOs.Cargos
{
    // [Diana desde v4.0 OBSOLETO] DTO de Cargo reemplazado por atributos en CreateDetalleDesignacionDto/DetalleDesignacionDto.
    // Mantener hasta completar el refactor completo.
    [Obsolete("DTO de Cargo obsoleto desde v4.0. Los atributos de cargo pasan a DetalleDesignacion.")]
    public class ListCargoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty; // "Titular Adjunto Docencia Ordinario"
        public float PuntosBase { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }
}