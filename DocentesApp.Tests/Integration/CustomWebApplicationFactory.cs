using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DocentesApp.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private DbConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover la configuración original del DbContext
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DocentesDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            var dbContextConfigDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbContextOptionsConfiguration<DocentesDbContext>));

            if (dbContextConfigDescriptor is not null)
            {
                services.Remove(dbContextConfigDescriptor);
            }

            var connectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbConnection));

            if (connectionDescriptor is not null)
            {
                services.Remove(connectionDescriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddSingleton<DbConnection>(_connection);

            services.AddDbContext<DocentesDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();

            db.Database.EnsureCreated();
            SeedData(db);
        });
    }

    private static void SeedData(DocentesDbContext db)
    {
        if (db.Docentes.Any())
            return;

        db.Docentes.AddRange(
            new Docente
            {
                Nombre = "Juan",
                Apellido = "Perez",
                Legajo = 12345,
                Dni = "11.111.111",
                Email = "juan@test.com",
                Observaciones = "Docente inicial"
            },
            new Docente
            {
                Nombre = "Ana",
                Apellido = "Gomez",
                Legajo = 45678,
                Dni = "22.222.222",
                Email = "ana@test.com",
                Observaciones = "Otra docente"
            });

        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}