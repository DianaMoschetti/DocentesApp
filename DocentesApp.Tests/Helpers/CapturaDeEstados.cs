using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DocentesApp.Tests.Helpers
{
    // Interceptor de test: guarda qué entradas iban a escribirse en cada SaveChanges (tipo, estado y propiedades
    // marcadas como modificadas), tal como las dejó el servicio, antes de que la auditoría las toque.
    public sealed class CapturaDeEstados : SaveChangesInterceptor
    {
        public sealed record Entrada(Type Tipo, EntityState Estado, IReadOnlyList<string> PropiedadesModificadas);

        public List<Entrada> Entradas { get; } = new();

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            Capturar(eventData.Context);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            Capturar(eventData.Context);
            return ValueTask.FromResult(result);
        }

        private void Capturar(DbContext? context)
        {
            if (context == null)
                return;

            context.ChangeTracker.DetectChanges();
            // Sin las filas de historial (las guarda la auditoría en un segundo SaveChanges)
            Entradas.AddRange(context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted
                    && e.Entity is not HistorialCambio and not HistorialCambioDetalle)
                .Select(e => new Entrada(
                    e.Metadata.ClrType,
                    e.State,
                    e.Properties.Where(p => p.IsModified).Select(p => p.Metadata.Name).ToList())));
        }
    }
}
