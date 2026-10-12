using DocentesApp.API.Extensions;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Seeds
{
    public class SeedAsignaturasTests : IDisposable
    {
        private readonly SqliteTestDatabase _db = new();

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task SeedAsignaturasAsync_NoDuplicaExistentes_MismaUdbYNombreConDistintasMayusculas()
        {
            int udbFisica;
            using (var context = _db.CrearContexto())
            {
                var fisica = new Udb { Nombre = "UDB Física" };
                context.Udbs.AddRange(fisica, new Udb { Nombre = "UDB Matemática" });
                await context.SaveChangesAsync();
                udbFisica = fisica.Id;

                // Cargada a mano, con otro uso de mayúsculas y otro nivel: no se toca ni se duplica
                context.Asignaturas.Add(new Asignatura
                {
                    Nombre = "FÍSICA I",
                    Nivel = Nivel.Segundo,
                    Frecuencia = Frecuencia.Cuatrimestral,
                    UdbId = fisica.Id
                });
                await context.SaveChangesAsync();
            }

            var app = CrearApp();
            await app.SeedAsignaturasAsync();
            await app.SeedAsignaturasAsync(); // segunda ejecución (como cada arranque de la API): no inserta nada

            using var verificacion = _db.CrearContexto();
            var deFisica = await verificacion.Asignaturas.Where(a => a.UdbId == udbFisica).ToListAsync();

            deFisica.Select(a => a.Nombre).Should().BeEquivalentTo("FÍSICA I", "Física II", "Taller de Óptica Geométrica");
            var existente = deFisica.Single(a => a.Nombre == "FÍSICA I");
            existente.Nivel.Should().Be(Nivel.Segundo);
            existente.Frecuencia.Should().Be(Frecuencia.Cuatrimestral);

            // Las UDB que no existen se omiten (LogWarning); las que existen reciben sus asignaturas
            (await verificacion.Asignaturas.CountAsync()).Should().Be(3 + 4);
        }

        // IApplicationBuilder mínimo con lo que usa el seed: DocentesDbContext y logging
        private IApplicationBuilder CrearApp()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddScoped(_ => new DocentesDbContext(_db.Options));

            return new ApplicationBuilder(services.BuildServiceProvider());
        }
    }
}
