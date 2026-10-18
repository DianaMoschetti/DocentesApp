using DocentesApp.Application.Interfaces.Auth;
using DocentesApp.Data.Auditoria;
using DocentesApp.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DocentesApp.Tests.Helpers
{
    // Base SQLite en memoria para tests de servicios v5.
    // A diferencia de UseInMemoryDatabase, SQLite es relacional: respeta FKs, índices únicos filtrados
    // (Legajo, DNI) y AsSplitQuery, y aplica el HasData de PuntosPorCargo (6 filas del catálogo).
    // Cada CrearContexto() devuelve un contexto nuevo sobre la misma conexión (como un request nuevo en la API).
    // Auditoría (opcional): con un usuario actual, cada contexto lleva su propio AuditoriaInterceptor
    // (como el scope de un request); sin usuario, los contextos no auditan.
    public sealed class SqliteTestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<DocentesDbContext> _options;

        // [Diana desde v4.0 OBSOLETO] sin opción de auditoría
        //public SqliteTestDatabase()
        public SqliteTestDatabase(ICurrentUserService? usuarioAuditoria = null)
        {
            UsuarioAuditoria = usuarioAuditoria;

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<DocentesDbContext>()
                .UseSqlite(_connection)
                .Options;

            // [Diana desde v4.0 OBSOLETO] CrearContexto() puede venir con auditoría; EnsureCreated no la necesita
            //using var context = CrearContexto();
            using var context = new DocentesDbContext(_options);
            context.Database.EnsureCreated();
        }

        public DbContextOptions<DocentesDbContext> Options => _options;

        // Usuario actual de la auditoría; null = contextos sin auditoría
        public ICurrentUserService? UsuarioAuditoria { get; }

        public SqliteConnection Connection => _connection;

        // [Diana desde v4.0 OBSOLETO] siempre sin auditoría
        //public DocentesDbContext CrearContexto() => new DocentesDbContext(_options);
        public DocentesDbContext CrearContexto() =>
            UsuarioAuditoria == null
                ? new DocentesDbContext(_options)
                : CrearContextoAuditado(new AuditoriaInterceptor(UsuarioAuditoria, new ContextoOperacionAuditoria()));

        // [Diana desde v4.0 OBSOLETO] sin interceptores previos
        //public DocentesDbContext CrearContextoAuditado(AuditoriaInterceptor interceptor) =>
        //    new DocentesDbContext(new DbContextOptionsBuilder<DocentesDbContext>()
        //        .UseSqlite(_connection)
        //        .AddInterceptors(interceptor)
        //        .Options);
        // Contexto con un interceptor dado (ej. compartir el ContextoOperacionAuditoria con IAuditoriaEventos, o forzar fallas).
        // "antes": interceptores que corren antes que la auditoría (ej. uno que captura los estados de las entradas)
        public DocentesDbContext CrearContextoAuditado(AuditoriaInterceptor interceptor, params IInterceptor[] antes) =>
            new DocentesDbContext(new DbContextOptionsBuilder<DocentesDbContext>()
                .UseSqlite(_connection)
                .AddInterceptors(antes.Append(interceptor))
                .Options);

        // Contexto auditado con el usuario de esta base y otros interceptores antes de la auditoría
        public DocentesDbContext CrearContexto(params IInterceptor[] antes) =>
            CrearContextoAuditado(new AuditoriaInterceptor(
                UsuarioAuditoria ?? throw new InvalidOperationException("La base no tiene usuario de auditoría."),
                new ContextoOperacionAuditoria()), antes);

        public void Dispose() => _connection.Dispose();
    }
}
