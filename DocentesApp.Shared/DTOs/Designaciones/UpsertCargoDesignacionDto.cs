using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Designaciones
{
    // Cargo de una designación, para alta y edición. Id null = cargo nuevo.
    // No lleva Puntos: los calcula y guarda el backend.
    public class UpsertCargoDesignacionDto
    {
        public int? Id { get; set; }
        public CargoDocente Cargo { get; set; }
        public Condicion Condicion { get; set; }
        public TipoDedicacion TipoDedicacion { get; set; }
        public decimal CantidadDedicacion { get; set; } = 1; // 0,5 / 1 / 1,5 ...
        public int UdbId { get; set; }
        public bool EnLicencia { get; set; }
        public int? ReemplazaACargoId { get; set; } // solo con Condicion = Suplente
        public string? Observaciones { get; set; }
        public List<UpsertActividadDto> Actividades { get; set; } = new();
    }
}
