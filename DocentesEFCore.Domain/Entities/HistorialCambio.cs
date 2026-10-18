using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Entities
{
    // Historial de cambios (auditoría): una fila por entidad cambiada en un SaveChanges (o por evento explícito).
    // Sin FKs a las tablas de negocio: la fila sobrevive aunque la entidad (o el usuario) se borre.
    public class HistorialCambio
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; } // UTC en BD; la UI muestra hora Argentina
        public Guid OperacionId { get; set; } // agrupa los cambios de una misma operación (transacción)

        // Autor: null = "Sistema" (sin usuario, ej. seed). Nombre y email son una copia al momento del cambio.
        public string? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? UsuarioEmail { get; set; }

        public string Entidad { get; set; } = null!; // nombre del tipo, ej "Docente"
        public string EntidadId { get; set; } = null!; // texto; clave compuesta como "DocenteId=1;UdbId=2"
        public string? EntidadDescripcion { get; set; } // copia descriptiva de la entidad al momento del cambio
        public AccionAuditoria Accion { get; set; }

        // Vínculos para el historial por docente / por designación (sin FK)
        public int? DocenteId { get; set; }
        public int? DesignacionId { get; set; }

        public ICollection<HistorialCambioDetalle> Detalles { get; set; } = new List<HistorialCambioDetalle>();
    }
}
