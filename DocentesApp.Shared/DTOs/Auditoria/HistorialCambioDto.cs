using DocentesApp.Domain.Enums;

namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Una fila del historial de cambios, lista para mostrar (etiquetas en castellano, FKs y enums por nombre)
    public class HistorialCambioDto
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; }
        public DateTime FechaArgentina { get; set; }
        public Guid OperacionId { get; set; } // agrupa los cambios de una misma operación

        public string? UsuarioId { get; set; } // null = Sistema
        public string UsuarioNombre { get; set; } = string.Empty; // "Sistema" si no hubo usuario
        public string? UsuarioEmail { get; set; }

        public string Entidad { get; set; } = string.Empty;       // clave guardada (ej. "CargoDesignacion"), la que usa el filtro
        public string EntidadNombre { get; set; } = string.Empty; // etiqueta (ej. "Cargo de designación")
        public string EntidadId { get; set; } = string.Empty;
        public string? EntidadDescripcion { get; set; }

        public AccionAuditoria Accion { get; set; }
        public string AccionNombre { get; set; } = string.Empty; // Alta / Modificación / Baja

        public int? DocenteId { get; set; }
        public int? DesignacionId { get; set; }

        public List<HistorialCambioDetalleDto> Detalles { get; set; } = new();
    }
}
