using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DocentesApp.Data.Auditoria
{
    // Operación de auditoría en curso (scoped): agrupa con un mismo OperacionId todo lo que se guarda en una transacción.
    // Con una transacción abierta, cada SaveChanges y cada evento explícito (IAuditoriaEventos) dentro de ella
    // reusa el mismo OperacionId (ej. crear usuario + asignar rol = una operación). Sin transacción, uno nuevo cada vez.
    public class ContextoOperacionAuditoria
    {
        private DbTransaction? _transaccion;
        private Guid _operacionId;

        public Guid ObtenerOperacionId(DbContext context)
        {
            var transaccion = context.Database.CurrentTransaction?.GetDbTransaction();
            if (transaccion == null)
                return Guid.NewGuid();

            if (!ReferenceEquals(transaccion, _transaccion))
            {
                _transaccion = transaccion;
                _operacionId = Guid.NewGuid();
            }

            return _operacionId;
        }
    }
}
