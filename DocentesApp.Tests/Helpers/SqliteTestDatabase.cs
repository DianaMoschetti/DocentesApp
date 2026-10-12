using DocentesApp.Data.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Tests.Helpers
{
    // Base SQLite en memoria para tests de servicios v5.
    // A diferencia de UseInMemoryDatabase, SQLite es relacional: respeta FKs, índices únicos filtrados
    // (Legajo, DNI) y AsSplitQuery, y aplica el HasData de PuntosPorCargo (6 filas del catálogo).
    // Cada CrearContexto() devuelve un contexto nuevo sobre la misma conexión (como un request nuevo en la API).
    public sealed class SqliteTestDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<DocentesDbContext> _options;

        public SqliteTestDatabase()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<DocentesDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var context = CrearContexto();
            context.Database.EnsureCreated();
        }

        public DbContextOptions<DocentesDbContext> Options => _options;

        public DocentesDbContext CrearContexto() => new DocentesDbContext(_options);

        public void Dispose() => _connection.Dispose();
    }
}
