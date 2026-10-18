using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Udbs;
using DocentesApp.Tests.Helpers;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Tests.Services
{
    // UdbService contra SQLite en memoria, con el interceptor de auditoría.
    // Los datos previos se cargan sin auditoría; cada operación usa un contexto nuevo (como un request).
    public class UdbServiceTests : IDisposable
    {
        private static readonly DateTimeOffset FechaPrevia = new(2025, 3, 1, 12, 0, 0, TimeSpan.Zero);

        private readonly SqliteTestDatabase _db = new(UsuarioActualFalso.Editor());
        private readonly MapsterMapper.IMapper _mapper = MapperFactory.Create();

        private readonly int _director;
        private readonly int _secretario;
        private readonly int _otroDocente;
        private readonly int _udb;

        public UdbServiceTests()
        {
            using var context = new DocentesDbContext(_db.Options);

            Docente Docente(string apellido, string dni) => new()
            {
                Nombre = "N", Apellido = apellido, Dni = dni,
                CreatedBy = "creador", CreatedDate = FechaPrevia, ModifiedBy = "creador", ModifiedDate = FechaPrevia
            };
            var director = Docente("Director", "1");
            var secretario = Docente("Secretario", "2");
            var otro = Docente("Otro", "3");
            context.AddRange(director, secretario, otro);
            context.SaveChanges();

            var udb = new Udb
            {
                Nombre = "UDB Física", DirectorDocenteId = director.Id, SecretarioDocenteId = secretario.Id,
                CreatedBy = "creador", CreatedDate = FechaPrevia, ModifiedBy = "creador", ModifiedDate = FechaPrevia
            };
            context.Udbs.Add(udb);
            context.SaveChanges();

            _director = director.Id;
            _secretario = secretario.Id;
            _otroDocente = otro.Id;
            _udb = udb.Id;
        }

        public void Dispose() => _db.Dispose();

        private List<HistorialCambio> Historial()
        {
            using var context = new DocentesDbContext(_db.Options);
            return context.HistorialCambios.Include(h => h.Detalles).AsNoTracking().OrderBy(h => h.Id).ToList();
        }

        private async Task ActualizarAsync(UpdateUdbDto dto, CapturaDeEstados? captura = null)
        {
            await using var context = captura == null ? _db.CrearContexto() : _db.CrearContexto(captura);
            await new UdbService(new UdbRepository(context), _mapper).UpdateAsync(_udb, dto);
        }

        private UpdateUdbDto Dto(string nombre = "UDB Física", int? director = -1, int? secretario = -1) => new()
        {
            Nombre = nombre,
            DirectorDocenteId = director == -1 ? _director : director,
            SecretarioDocenteId = secretario == -1 ? _secretario : secretario
        };

        [Fact]
        public async Task UpdateAsync_SoloEscribeLaUdb_NoMarcaModificadosAlDirectorNiAlSecretario()
        {
            var captura = new CapturaDeEstados();

            await ActualizarAsync(Dto(nombre: "UDB Física II"), captura);

            // Solo la UDB, y solo la columna que cambió (sin Update(): ni el grafo ni todas las columnas)
            var entrada = captura.Entradas.Should().ContainSingle().Subject;
            entrada.Tipo.Should().Be(typeof(Udb));
            entrada.Estado.Should().Be(EntityState.Modified);
            entrada.PropiedadesModificadas.Should().BeEquivalentTo(new[] { "Nombre" });

            var fila = Historial().Should().ContainSingle().Subject;
            fila.Entidad.Should().Be("Udb");
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Nombre" && d.ValorAnterior == "UDB Física" && d.ValorNuevo == "UDB Física II");

            using var lectura = new DocentesDbContext(_db.Options);
            lectura.Docentes.Should().OnlyContain(d => d.ModifiedBy == "creador" && d.ModifiedDate == FechaPrevia);
            var udb = lectura.Udbs.Single(u => u.Id == _udb);
            udb.CreatedBy.Should().Be("creador");
            udb.CreatedDate.Should().Be(FechaPrevia);
            udb.ModifiedBy.Should().Be("user-editor");
        }

        [Fact]
        public async Task UpdateAsync_CambiaDirectorYQuitaSecretario_GuardaSoloLasFks()
        {
            var captura = new CapturaDeEstados();

            await ActualizarAsync(Dto(director: _otroDocente, secretario: null), captura);

            captura.Entradas.Should().ContainSingle(e => e.Tipo == typeof(Udb))
                .Which.PropiedadesModificadas.Should().BeEquivalentTo(new[] { "DirectorDocenteId", "SecretarioDocenteId" });
            captura.Entradas.Should().NotContain(e => e.Tipo == typeof(Docente));

            using var lectura = new DocentesDbContext(_db.Options);
            var udb = lectura.Udbs.Single(u => u.Id == _udb);
            udb.DirectorDocenteId.Should().Be(_otroDocente);
            udb.SecretarioDocenteId.Should().BeNull();
            Historial().Single().Detalles.Select(d => (d.Propiedad, d.ValorAnterior, d.ValorNuevo)).Should().BeEquivalentTo(new[]
            {
                ("DirectorDocenteId", (string?)_director.ToString(), (string?)_otroDocente.ToString()),
                ("SecretarioDocenteId", (string?)_secretario.ToString(), (string?)null)
            });
        }

        [Fact]
        public async Task UpdateAsync_SinCambios_NoEscribeNada()
        {
            var captura = new CapturaDeEstados();

            await ActualizarAsync(Dto(), captura);

            captura.Entradas.Should().BeEmpty();
            Historial().Should().BeEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateAsync_ConDirectorOSecretarioInexistente_Lanza400_YNoGuarda(bool director)
        {
            var dto = director ? Dto(director: 9999) : Dto(secretario: 9999);

            var act = () => ActualizarAsync(dto);

            (await act.Should().ThrowAsync<BadRequestException>()).WithMessage("*9999*");
            using var lectura = new DocentesDbContext(_db.Options);
            var udb = lectura.Udbs.Single(u => u.Id == _udb);
            udb.DirectorDocenteId.Should().Be(_director);
            udb.SecretarioDocenteId.Should().Be(_secretario);
            Historial().Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateAsync_UdbInexistente_Lanza404()
        {
            await using var context = _db.CrearContexto();
            var service = new UdbService(new UdbRepository(context), _mapper);

            var act = () => service.UpdateAsync(9999, Dto());

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_BorraLasMembresias_YQuedanEnElHistorial()
        {
            using (var context = new DocentesDbContext(_db.Options))
            {
                context.DocenteUdbs.Add(new DocenteUdb { DocenteId = _otroDocente, UdbId = _udb, Rol = RolUdb.Docente });
                context.SaveChanges();
            }

            await using (var context = _db.CrearContexto())
                await new UdbService(new UdbRepository(context), _mapper).DeleteAsync(_udb);

            var filas = Historial();
            filas.Select(f => (f.Entidad, f.Accion)).Should().BeEquivalentTo(new[]
            {
                ("Udb", AccionAuditoria.Baja),
                ("DocenteUdb", AccionAuditoria.Baja)
            });
            filas.Single(f => f.Entidad == "DocenteUdb").DocenteId.Should().Be(_otroDocente);
            using var lectura = new DocentesDbContext(_db.Options);
            lectura.DocenteUdbs.Should().BeEmpty();
        }
    }
}
