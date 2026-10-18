using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Asignaturas;
using DocentesApp.Shared.DTOs.Cursos;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Tests.Helpers;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Tests.Services
{
    // DocenteService, AsignaturaService y CursoService con el interceptor de auditoría (etapa 4a):
    //   - editar sin Update(): solo se escriben las columnas que cambian, y nada si no cambia nada
    //   - borrar con los hijos cargados: la cascada (DocenteUdb, AsignaturaModulo) queda en el historial
    public class EdicionesYBorradosAuditadosTests : IDisposable
    {
        private readonly SqliteTestDatabase _db = new(UsuarioActualFalso.Editor());
        private readonly MapsterMapper.IMapper _mapper = MapperFactory.Create();

        private readonly int _docente;
        private readonly int _udb;
        private readonly int _asignatura;
        private readonly int _curso;

        public EdicionesYBorradosAuditadosTests()
        {
            using var context = new DocentesDbContext(_db.Options);
            var docente = new Docente { Nombre = "Juan", Apellido = "Perez", Dni = "1", Legajo = 1, Email = "juan@test.com" };
            var udb = new Udb { Nombre = "UDB Física" };
            context.AddRange(docente, udb);
            context.SaveChanges();

            var asignatura = new Asignatura { Nombre = "Física I", Nivel = Nivel.Primero, Frecuencia = Frecuencia.Anual, UdbId = udb.Id };
            var curso = new Curso { Turno = Turno.Mañana, Año = Nivel.Primero, Carrera = Especialidad.ISI, NroComision = 1 };
            context.AddRange(asignatura, curso);
            context.DocenteUdbs.Add(new DocenteUdb { DocenteId = docente.Id, UdbId = udb.Id, Rol = RolUdb.Docente });
            context.SaveChanges();

            _docente = docente.Id;
            _udb = udb.Id;
            _asignatura = asignatura.Id;
            _curso = curso.Id;
        }

        public void Dispose() => _db.Dispose();

        private List<HistorialCambio> Historial()
        {
            using var context = new DocentesDbContext(_db.Options);
            return context.HistorialCambios.Include(h => h.Detalles).AsNoTracking().OrderBy(h => h.Id).ToList();
        }

        private void AgregarModulo()
        {
            using var context = new DocentesDbContext(_db.Options);
            context.AsignaturaModulos.Add(new AsignaturaModulo { AsignaturaId = _asignatura, CursoId = _curso, Modulo = default });
            context.SaveChanges();
        }

        #region Docente

        [Fact]
        public async Task Docente_UpdateContacto_SoloEscribeLaColumnaQueCambia()
        {
            var captura = new CapturaDeEstados();

            await using (var context = _db.CrearContexto(captura))
                await new DocenteService(new DocenteRepository(context), _mapper).UpdateContactoAsync(_docente,
                    new UpdateContactoDocenteDto { Email = "juan.perez@test.com" });

            captura.Entradas.Should().ContainSingle().Which.PropiedadesModificadas.Should().BeEquivalentTo(new[] { "Email" });
            Historial().Should().ContainSingle().Which.Detalles.Should()
                .ContainSingle(d => d.Propiedad == "Email" && d.ValorAnterior == "juan@test.com" && d.ValorNuevo == "juan.perez@test.com");
        }

        [Fact]
        public async Task Docente_UpdateSinCambios_NoEscribeNada()
        {
            var captura = new CapturaDeEstados();

            await using (var context = _db.CrearContexto(captura))
                await new DocenteService(new DocenteRepository(context), _mapper).UpdateAsync(_docente, new UpdateDocenteDto
                {
                    Nombre = "Juan", Apellido = "Perez", Dni = "1", Legajo = 1, Email = "juan@test.com", Observaciones = string.Empty
                });

            captura.Entradas.Should().BeEmpty();
            Historial().Should().BeEmpty();
        }

        [Fact]
        public async Task Docente_Delete_RegistraTambienLaBajaDeSusMembresias()
        {
            await using (var context = _db.CrearContexto())
                await new DocenteService(new DocenteRepository(context), _mapper).DeleteAsync(_docente);

            var filas = Historial();
            filas.Select(f => (f.Entidad, f.Accion)).Should().BeEquivalentTo(new[]
            {
                ("Docente", AccionAuditoria.Baja),
                ("DocenteUdb", AccionAuditoria.Baja)
            });
            filas.Should().OnlyContain(f => f.DocenteId == _docente);
            filas.Select(f => f.OperacionId).Distinct().Should().ContainSingle();
        }

        #endregion

        #region Asignatura

        [Fact]
        public async Task Asignatura_Update_NoMarcaLaUdbComoModificada_YSoloEscribeLoQueCambia()
        {
            var captura = new CapturaDeEstados();

            await using (var context = _db.CrearContexto(captura))
                await new AsignaturaService(new AsignaturaRepository(context), _mapper).UpdateAsync(_asignatura, new UpdateAsignaturaDto
                {
                    Nombre = "Física I", Nivel = Nivel.Primero, Frecuencia = Frecuencia.Anual, UdbId = _udb, EsVigente = false
                });

            var entrada = captura.Entradas.Should().ContainSingle().Subject;
            entrada.Tipo.Should().Be(typeof(Asignatura));
            entrada.PropiedadesModificadas.Should().BeEquivalentTo(new[] { "EsVigente" });
            Historial().Should().ContainSingle().Which.Entidad.Should().Be("Asignatura");
        }

        [Fact]
        public async Task Asignatura_Delete_RegistraTambienLaBajaDeSusModulos()
        {
            AgregarModulo();

            await using (var context = _db.CrearContexto())
                await new AsignaturaService(new AsignaturaRepository(context), _mapper).DeleteAsync(_asignatura);

            Historial().Select(f => (f.Entidad, f.Accion)).Should().BeEquivalentTo(new[]
            {
                ("Asignatura", AccionAuditoria.Baja),
                ("AsignaturaModulo", AccionAuditoria.Baja)
            });
        }

        #endregion

        #region Curso

        [Fact]
        public async Task Curso_UpdateSinCambios_NoEscribeNada()
        {
            var captura = new CapturaDeEstados();

            await using (var context = _db.CrearContexto(captura))
                await new CursoService(new CursoRepository(context), _mapper).UpdateAsync(_curso, new UpdateCursoDto
                {
                    Turno = (int)Turno.Mañana, Año = (int)Nivel.Primero, Carrera = (int)Especialidad.ISI, NroComision = 1
                });

            captura.Entradas.Should().BeEmpty();
            Historial().Should().BeEmpty();
        }

        // Un curso con módulos no se puede borrar (HasDependenciasAsync -> 409): la carga de módulos antes del
        // Remove no cambia nada hoy, pero mantiene la regla si esa validación cambia
        [Fact]
        public async Task Curso_ConModulos_NoSeBorra_YNoRegistraNada()
        {
            AgregarModulo();

            await using var context = _db.CrearContexto();
            var act = () => new CursoService(new CursoRepository(context), _mapper).DeleteAsync(_curso);

            await act.Should().ThrowAsync<DocentesApp.Application.Common.Exceptions.ConflictException>();
            Historial().Should().BeEmpty();
        }

        [Fact]
        public async Task Curso_Delete_RegistraLaBaja()
        {
            await using (var context = _db.CrearContexto())
                await new CursoService(new CursoRepository(context), _mapper).DeleteAsync(_curso);

            Historial().Should().ContainSingle(f => f.Entidad == "Curso" && f.Accion == AccionAuditoria.Baja);
        }

        #endregion
    }
}
