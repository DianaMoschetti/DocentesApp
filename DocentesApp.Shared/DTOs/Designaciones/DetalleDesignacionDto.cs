// [Diana desde v4.0 OBSOLETO] archivo completo comentado: reemplazado por CargoDesignacionDto + ActividadDto (v5).
//using DocentesApp.Domain.Enums;
//namespace DocentesApp.Shared.DTOs.Designaciones
//{
//    // [Diana desde v4.0 OBSOLETO] reemplazado por CargoDesignacionDto + ActividadDto
//    [Obsolete("Usar CargoDesignacionDto y ActividadDto.")]
//    public class DetalleDesignacionDto
//    {
//        public int Id { get; set; }
//        public int DesignacionId { get; set; }
//        public DenominacionCargo Denominacion { get; set; }
//        public TipoCargo TipoCargo { get; set; }
//        public Condicion Condicion { get; set; }
//        public TipoDedicacion TipoDedicacion { get; set; }
//        public float CantidadDedicacion { get; set; }
//        public EspecificacionCargo Especificacion { get; set; }
//        public int? AsignaturaId { get; set; }
//        public string? NombreAsignatura { get; set; }
//        public int? CursoId { get; set; }
//        public string? DescripcionCurso { get; set; }
//        // [Diana desde v4.0 OBSOLETO] PuntosUtilizados renombrado a PuntosAsignados
//        // public decimal? PuntosUtilizados { get; set; }
//        public decimal? PuntosAsignados { get; set; }
//        public string? Observaciones { get; set; }
//    }
//}