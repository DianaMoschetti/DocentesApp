using DocentesApp.Application.Interfaces.Auth;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Data.Auditoria
{
    public class AuditoriaEventos : IAuditoriaEventos
    {
        private readonly DocentesDbContext _context;
        private readonly ICurrentUserService _usuarioActual;
        private readonly ContextoOperacionAuditoria _contextoOperacion;

        public AuditoriaEventos(DocentesDbContext context, ICurrentUserService usuarioActual, ContextoOperacionAuditoria contextoOperacion)
        {
            _context = context;
            _usuarioActual = usuarioActual;
            _contextoOperacion = contextoOperacion;
        }

        public async Task RegistrarAsync(
            string entidad,
            string entidadId,
            string? entidadDescripcion,
            AccionAuditoria accion,
            IEnumerable<DetalleEventoAuditoria> detalles,
            int? docenteId = null,
            int? designacionId = null,
            CancellationToken cancellationToken = default)
        {
            var fila = new HistorialCambio
            {
                FechaUtc = DateTime.UtcNow,
                OperacionId = _contextoOperacion.ObtenerOperacionId(_context),
                UsuarioId = _usuarioActual.UserId,
                UsuarioNombre = _usuarioActual.Nombre,
                UsuarioEmail = _usuarioActual.Email,
                Entidad = entidad,
                EntidadId = entidadId,
                EntidadDescripcion = entidadDescripcion,
                Accion = accion,
                DocenteId = docenteId,
                DesignacionId = designacionId,
                Detalles = detalles
                    .Select(d => new HistorialCambioDetalle { Propiedad = d.Propiedad, ValorAnterior = d.ValorAnterior, ValorNuevo = d.ValorNuevo })
                    .ToList()
            };

            _context.HistorialCambios.Add(fila);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
