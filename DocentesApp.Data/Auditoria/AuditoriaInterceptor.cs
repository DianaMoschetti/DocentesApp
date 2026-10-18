using System.Globalization;
using System.Runtime.CompilerServices;
using DocentesApp.Application.Interfaces.Auth;
using DocentesApp.Data.Identity;
using DocentesApp.Domain.Base;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace DocentesApp.Data.Auditoria
{
    // Historial de cambios: completa las columnas de auditoría (IAuditable) y escribe un HistorialCambio
    // (con sus detalles) por cada entidad dada de alta, modificada o borrada en un SaveChanges.
    //   - SavingChanges: arma las filas de Modificación/Baja (valores originales; vínculo con docente/designación
    //     resuelto antes del guardado, cuando los principales todavía existen) y abre una transacción si no hay una.
    //   - SavedChanges: arma las Altas (ya con Id), guarda el historial en un segundo SaveChanges y confirma la transacción propia.
    //   - SaveChangesFailed: deshace la transacción propia.
    // Si falla el guardado del historial, la transacción propia se deshace: tampoco queda el cambio de negocio.
    // Scoped (una instancia por request); el estado se guarda por DbContext.
    public class AuditoriaInterceptor : SaveChangesInterceptor
    {
        public const string UsuarioSistema = "Sistema";

        private const int LargoUsuarioNombre = 256;
        private const int LargoUsuarioEmail = 256;
        private const int LargoEntidadDescripcion = 500;

        // Columnas de auditoría: nunca van al detalle
        private static readonly HashSet<string> ColumnasAuditoria = new()
        {
            nameof(IAuditable.CreatedDate), nameof(IAuditable.CreatedBy),
            nameof(IAuditable.ModifiedDate), nameof(IAuditable.ModifiedBy)
        };

        // Campos técnicos de Identity: nunca se guardan (tampoco en las altas) y cambiarlos solos no es una modificación
        private static readonly HashSet<string> CamposTecnicosUsuario = new()
        {
            nameof(ApplicationUser.PasswordHash), nameof(ApplicationUser.SecurityStamp), nameof(ApplicationUser.ConcurrencyStamp),
            nameof(ApplicationUser.AccessFailedCount), nameof(ApplicationUser.LockoutEnd), nameof(ApplicationUser.LockoutEnabled),
            nameof(ApplicationUser.NormalizedEmail), nameof(ApplicationUser.NormalizedUserName)
        };

        // Tablas que no se auditan: el propio historial y las de Identity salvo los usuarios
        // (roles y contraseñas se registran como eventos explícitos, ver IAuditoriaEventos)
        private static readonly HashSet<Type> TiposIgnorados = new()
        {
            typeof(HistorialCambio), typeof(HistorialCambioDetalle),
            typeof(IdentityUserRole<string>), typeof(IdentityRoleClaim<string>), typeof(IdentityUserClaim<string>),
            typeof(IdentityUserLogin<string>), typeof(IdentityUserToken<string>)
        };

        private readonly ICurrentUserService _usuarioActual;
        private readonly ContextoOperacionAuditoria _contextoOperacion;
        private readonly ConditionalWeakTable<DbContext, Estado> _estados = new();

        public AuditoriaInterceptor(ICurrentUserService usuarioActual, ContextoOperacionAuditoria contextoOperacion)
        {
            _usuarioActual = usuarioActual;
            _contextoOperacion = contextoOperacion;
        }

        // Estado de un guardado en curso, por DbContext
        private sealed class Estado
        {
            public bool GuardandoHistorial; // guardia de reentrada: el segundo SaveChanges solo lleva el historial
            public IDbContextTransaction? TransaccionPropia;
            public Guid OperacionId;
            public DateTimeOffset Fecha;
            public readonly List<HistorialCambio> Filas = new();
            public readonly List<EntityEntry> Altas = new();

            public void Limpiar()
            {
                Filas.Clear();
                Altas.Clear();
            }
        }

        #region Hooks (sync y async comparten la lógica; con async = false todo corre sincrónico)

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            AntesDeGuardarAsync(eventData.Context, async: false, default).GetAwaiter().GetResult();
            return result;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await AntesDeGuardarAsync(eventData.Context, async: true, cancellationToken);
            return result;
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            DespuesDeGuardarAsync(eventData.Context, async: false, default).GetAwaiter().GetResult();
            return result;
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            await DespuesDeGuardarAsync(eventData.Context, async: true, cancellationToken);
            return result;
        }

        public override void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            AlFallarAsync(eventData.Context, async: false, default).GetAwaiter().GetResult();
        }

        public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default) =>
            AlFallarAsync(eventData.Context, async: true, cancellationToken);

        #endregion

        private async Task AntesDeGuardarAsync(DbContext? context, bool async, CancellationToken cancellationToken)
        {
            if (context == null)
                return;

            var estado = _estados.GetValue(context, _ => new Estado());
            if (estado.GuardandoHistorial)
                return;
            estado.Limpiar();

            context.ChangeTracker.DetectChanges();
            var entradas = context.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted && SeAudita(e))
                .ToList();
            if (entradas.Count == 0)
                return;

            var autoDetect = context.ChangeTracker.AutoDetectChangesEnabled;
            context.ChangeTracker.AutoDetectChangesEnabled = false; // ya se detectaron; evita repetirlo en cada búsqueda
            try
            {
                estado.Fecha = DateTimeOffset.UtcNow;
                var autor = _usuarioActual.UserId ?? UsuarioSistema;
                var resolutor = new ResolutorVinculos(context, async, cancellationToken);

                foreach (var entry in entradas)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            if (entry.Entity is IAuditable)
                            {
                                entry.Property(nameof(IAuditable.CreatedDate)).CurrentValue = estado.Fecha;
                                entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = autor;
                                entry.Property(nameof(IAuditable.ModifiedDate)).CurrentValue = estado.Fecha;
                                entry.Property(nameof(IAuditable.ModifiedBy)).CurrentValue = autor;
                            }
                            estado.Altas.Add(entry); // la fila se arma después del guardado, con el Id ya generado
                            break;

                        case EntityState.Modified:
                            var cambios = DetallesModificacion(entry);
                            if (entry.Entity is IAuditable)
                            {
                                // Created* nunca se pisa
                                DejarSinModificar(entry, nameof(IAuditable.CreatedDate));
                                DejarSinModificar(entry, nameof(IAuditable.CreatedBy));
                                if (cambios.Count == 0)
                                {
                                    // Update() sin cambios reales: sin fila y sin tocar Modified*
                                    DejarSinModificar(entry, nameof(IAuditable.ModifiedDate));
                                    DejarSinModificar(entry, nameof(IAuditable.ModifiedBy));
                                }
                                else
                                {
                                    entry.Property(nameof(IAuditable.ModifiedDate)).CurrentValue = estado.Fecha;
                                    entry.Property(nameof(IAuditable.ModifiedBy)).CurrentValue = autor;
                                }
                            }
                            if (cambios.Count > 0)
                                estado.Filas.Add(await CrearFilaAsync(entry, AccionAuditoria.Modificacion, cambios, usarOriginales: true, resolutor, estado.Fecha));
                            break;

                        case EntityState.Deleted:
                            estado.Filas.Add(await CrearFilaAsync(entry, AccionAuditoria.Baja, DetallesBaja(entry), usarOriginales: true, resolutor, estado.Fecha));
                            break;
                    }
                }
            }
            finally
            {
                context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;
            }

            if (estado.Filas.Count == 0 && estado.Altas.Count == 0)
                return;

            // Negocio e historial en la misma transacción: si hay una abierta se usa; si no, se abre una propia
            if (context.Database.IsRelational() && context.Database.CurrentTransaction == null)
            {
                if (context.Database.CreateExecutionStrategy().RetriesOnFailure)
                    throw new InvalidOperationException(
                        "La auditoría abre una transacción en SaveChanges y la estrategia de ejecución configurada reintenta " +
                        "(EnableRetryOnFailure). Envolvé la operación en CreateExecutionStrategy().ExecuteAsync(...) con una " +
                        "transacción explícita, o quitá los reintentos (ver docs/auditoria-plan.md).");

                estado.TransaccionPropia = async
                    ? await context.Database.BeginTransactionAsync(cancellationToken)
                    : context.Database.BeginTransaction();
            }

            estado.OperacionId = _contextoOperacion.ObtenerOperacionId(context);
        }

        private async Task DespuesDeGuardarAsync(DbContext? context, bool async, CancellationToken cancellationToken)
        {
            if (context == null || !_estados.TryGetValue(context, out var estado) || estado.GuardandoHistorial)
                return;

            if (estado.Filas.Count == 0 && estado.Altas.Count == 0)
            {
                await TerminarTransaccionAsync(estado, confirmar: true, async, cancellationToken);
                return;
            }

            try
            {
                var autoDetect = context.ChangeTracker.AutoDetectChangesEnabled;
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                try
                {
                    var resolutor = new ResolutorVinculos(context, async, cancellationToken);
                    foreach (var entry in estado.Altas)
                        estado.Filas.Add(await CrearFilaAsync(entry, AccionAuditoria.Alta, DetallesAlta(entry), usarOriginales: false, resolutor, estado.Fecha));
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;
                }

                foreach (var fila in estado.Filas)
                    fila.OperacionId = estado.OperacionId;

                // El segundo SaveChanges solo debe llevar el historial (con acceptAllChangesOnSuccess: false quedarían
                // cambios de negocio pendientes y se guardarían dos veces)
                if (context.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
                    throw new InvalidOperationException("La auditoría no admite SaveChanges(acceptAllChangesOnSuccess: false).");

                estado.GuardandoHistorial = true;
                try
                {
                    await GuardarHistorialAsync(context, estado.Filas, async, cancellationToken);
                }
                finally
                {
                    estado.GuardandoHistorial = false;
                }

                // Ya guardadas: no quedan en el change tracker del llamador
                DesconectarFilas(context, estado.Filas);

                await TerminarTransaccionAsync(estado, confirmar: true, async, cancellationToken);
            }
            catch
            {
                // Historial no guardado: se descartan sus filas y se deshace la transacción propia (también el cambio de negocio).
                // Con una transacción del llamador, la excepción le llega y la deshace él.
                // [Diana desde v4.0 OBSOLETO] movido a DesconectarFilas (también se usa al guardar bien)
                //foreach (var fila in estado.Filas)
                //{
                //    // copia: al desconectar un detalle EF lo saca de fila.Detalles
                //    foreach (var detalle in fila.Detalles.ToList())
                //        context.Entry(detalle).State = EntityState.Detached;
                //    context.Entry(fila).State = EntityState.Detached;
                //}
                DesconectarFilas(context, estado.Filas);
                await TerminarTransaccionAsync(estado, confirmar: false, async, cancellationToken);
                throw;
            }
            finally
            {
                estado.Limpiar();
            }
        }

        private async Task AlFallarAsync(DbContext? context, bool async, CancellationToken cancellationToken)
        {
            // Si falló el guardado del historial (reentrada), lo maneja DespuesDeGuardarAsync
            if (context == null || !_estados.TryGetValue(context, out var estado) || estado.GuardandoHistorial)
                return;

            estado.Limpiar();
            await TerminarTransaccionAsync(estado, confirmar: false, async, cancellationToken);
        }

        // Saca las filas de historial (y sus detalles) del change tracker
        private static void DesconectarFilas(DbContext context, IEnumerable<HistorialCambio> filas)
        {
            foreach (var fila in filas)
            {
                // copia: al desconectar un detalle EF lo saca de fila.Detalles
                foreach (var detalle in fila.Detalles.ToList())
                    context.Entry(detalle).State = EntityState.Detached;
                context.Entry(fila).State = EntityState.Detached;
            }
        }

        // Segundo SaveChanges, solo con el historial. Virtual para que un test pueda forzar que falle.
        protected virtual async Task GuardarHistorialAsync(DbContext context, IReadOnlyList<HistorialCambio> filas, bool async, CancellationToken cancellationToken)
        {
            context.Set<HistorialCambio>().AddRange(filas);
            if (async)
                await context.SaveChangesAsync(cancellationToken);
            else
                context.SaveChanges();
        }

        // Confirma o deshace la transacción propia (si la hay) y siempre la libera
        private static async Task TerminarTransaccionAsync(Estado estado, bool confirmar, bool async, CancellationToken cancellationToken)
        {
            var transaccion = estado.TransaccionPropia;
            if (transaccion == null)
                return;
            estado.TransaccionPropia = null;

            try
            {
                if (confirmar)
                {
                    if (async) await transaccion.CommitAsync(cancellationToken);
                    else transaccion.Commit();
                }
                else
                {
                    try
                    {
                        if (async) await transaccion.RollbackAsync(CancellationToken.None);
                        else transaccion.Rollback();
                    }
                    catch
                    {
                        // No tapar la excepción original; al liberarla la base deshace la transacción igual
                    }
                }
            }
            finally
            {
                if (async) await transaccion.DisposeAsync();
                else transaccion.Dispose();
            }
        }

        #region Filas y detalles

        private async Task<HistorialCambio> CrearFilaAsync(
            EntityEntry entry, AccionAuditoria accion, List<HistorialCambioDetalle> detalles,
            bool usarOriginales, ResolutorVinculos resolutor, DateTimeOffset fecha)
        {
            var regla = ReglasAuditoria.Obtener(entry.Metadata.ClrType);
            var (docenteId, designacionId) = regla?.Vinculo != null
                ? await regla.Vinculo(new VinculoContexto(resolutor, entry, usarOriginales))
                : (null, null);

            return new HistorialCambio
            {
                FechaUtc = fecha.UtcDateTime,
                UsuarioId = _usuarioActual.UserId,
                UsuarioNombre = Truncar(_usuarioActual.Nombre, LargoUsuarioNombre),
                UsuarioEmail = Truncar(_usuarioActual.Email, LargoUsuarioEmail),
                Entidad = regla?.Entidad ?? entry.Metadata.ShortName(),
                EntidadId = Clave(entry, usarOriginales),
                EntidadDescripcion = Truncar(regla?.Descripcion(entry.Entity), LargoEntidadDescripcion),
                Accion = accion,
                DocenteId = docenteId,
                DesignacionId = designacionId,
                Detalles = detalles
            };
        }

        // Alta: todos los valores no nulos, salvo la clave generada por la base (ya está en EntidadId)
        private static List<HistorialCambioDetalle> DetallesAlta(EntityEntry entry) =>
            entry.Properties
                .Where(p => !SeIgnora(entry, p.Metadata)
                    && !(p.Metadata.IsPrimaryKey() && p.Metadata.ValueGenerated != ValueGenerated.Never))
                .Select(p => (p.Metadata.Name, Valor: Serializar(p.CurrentValue)))
                .Where(x => x.Valor != null)
                .Select(x => Detalle(x.Name, null, x.Valor))
                .ToList();

        // Modificación: solo las propiedades cuyo valor cambió (no alcanza IsModified: Update() marca todas)
        private static List<HistorialCambioDetalle> DetallesModificacion(EntityEntry entry) =>
            entry.Properties
                .Where(p => !SeIgnora(entry, p.Metadata)
                    && !p.Metadata.GetValueComparer().Equals(p.OriginalValue, p.CurrentValue))
                .Select(p => Detalle(p.Metadata.Name, Serializar(p.OriginalValue), Serializar(p.CurrentValue)))
                .ToList();

        // Baja: los valores que tenía
        private static List<HistorialCambioDetalle> DetallesBaja(EntityEntry entry) =>
            entry.Properties
                .Where(p => !SeIgnora(entry, p.Metadata))
                .Select(p => (p.Metadata.Name, Valor: Serializar(p.OriginalValue)))
                .Where(x => x.Valor != null)
                .Select(x => Detalle(x.Name, x.Valor, null))
                .ToList();

        private static HistorialCambioDetalle Detalle(string propiedad, string? anterior, string? nuevo) =>
            new() { Propiedad = propiedad, ValorAnterior = anterior, ValorNuevo = nuevo };

        private static bool SeAudita(EntityEntry entry)
        {
            var tipo = entry.Metadata.ClrType;
            return !TiposIgnorados.Contains(tipo) && !typeof(IdentityRole).IsAssignableFrom(tipo);
        }

        private static bool SeIgnora(EntityEntry entry, IProperty propiedad) =>
            (entry.Entity is IAuditable && ColumnasAuditoria.Contains(propiedad.Name))
            || (entry.Entity is ApplicationUser && CamposTecnicosUsuario.Contains(propiedad.Name));

        // Vuelve la propiedad a su valor original y la saca del UPDATE
        private static void DejarSinModificar(EntityEntry entry, string propiedad)
        {
            var p = entry.Property(propiedad);
            if (!p.IsModified)
                return;
            p.CurrentValue = p.OriginalValue;
            p.IsModified = false;
        }

        // "5" o, con clave compuesta, "DocenteId=1;UdbId=2"
        private static string Clave(EntityEntry entry, bool usarOriginales)
        {
            var propiedades = entry.Metadata.FindPrimaryKey()?.Properties;
            if (propiedades == null || propiedades.Count == 0)
                return string.Empty;

            string ValorDe(IProperty p)
            {
                var propiedad = entry.Property(p.Name);
                return Serializar(usarOriginales ? propiedad.OriginalValue : propiedad.CurrentValue) ?? string.Empty;
            }

            return propiedades.Count == 1
                ? ValorDe(propiedades[0])
                : string.Join(";", propiedades.Select(p => $"{p.Name}={ValorDe(p)}"));
        }

        // Serialización invariante; enums y FKs crudos (se traducen a nombres al leer)
        internal static string? Serializar(object? valor) => valor switch
        {
            null => null,
            string s => s,
            bool b => b ? "true" : "false",
            Enum e => Convert.ToInt64(e, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture),
            DateTime d => d.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture),
            DateTimeOffset d => d.ToString("O", CultureInfo.InvariantCulture),
            DateOnly d => d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            TimeOnly t => t.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture),
            IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
            _ => valor.ToString()
        };

        private static string? Truncar(string? texto, int largo) =>
            texto == null || texto.Length <= largo ? texto : texto[..largo];

        #endregion
    }
}
