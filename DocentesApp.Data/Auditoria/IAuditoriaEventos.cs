using DocentesApp.Domain.Enums;

namespace DocentesApp.Data.Auditoria
{
    // Detalle de un evento explícito (ej. Propiedad "Rol": "Editor" -> "Administrador"; "Contraseña" con ambos valores null)
    public sealed record DetalleEventoAuditoria(string Propiedad, string? ValorAnterior, string? ValorNuevo);

    // Eventos que no salen de un cambio de columna detectado por el interceptor: cambio de rol, reseteo y cambio de contraseña.
    // RegistrarAsync guarda su propia fila enseguida (SaveChangesAsync), dentro de la transacción del llamador si la hay
    // y con el mismo OperacionId que el resto de esa transacción. El interceptor ignora las filas de historial.
    public interface IAuditoriaEventos
    {
        Task RegistrarAsync(
            string entidad,
            string entidadId,
            string? entidadDescripcion,
            AccionAuditoria accion,
            IEnumerable<DetalleEventoAuditoria> detalles,
            int? docenteId = null,
            int? designacionId = null,
            CancellationToken cancellationToken = default);
    }
}
