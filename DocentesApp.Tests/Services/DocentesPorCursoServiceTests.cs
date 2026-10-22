using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Services;
using DocentesApp.Data.Context;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Reportes;
using DocentesApp.Tests.Helpers;
using AwesomeAssertions;

namespace DocentesApp.Tests.Services
{
    // Vista por curso contra SQLite en memoria: repositorio EF real + servicio.
    //
    // Física I (UDB Física):
    //   IQ 1° 101: Perez JTP (vigente), Mendez PT en licencia, Ibarra PT suplente de Mendez,
    //              Gomez (designación finalizada) y Lopez (FechaFin vencida) no cuentan
    //   IQ 1° 102: Diaz PAd (FechaFin futura)
    //   Cátedra: Ruiz PT Jefe de Cátedra y Ruiz JTP Laboratorio
    // Química (UDB Física):
    //   ISI 2° 201: Sosa PAd
    // Perez además tiene Gestión, Investigación en Física I sin curso y Otro sin curso: no aparecen.
    public class DocentesPorCursoServiceTests : IDisposable
    {
        private readonly SqliteTestDatabase _db = new();

        private readonly int _fisicaI;
        private readonly int _quimica;
        private readonly int _curso101;
        private readonly int _curso102;

        public DocentesPorCursoServiceTests()
        {
            using var context = new DocentesDbContext(_db.Options);

            var udb = new Udb { Nombre = "UDB Física" };
            context.Udbs.Add(udb);
            context.SaveChanges();

            var fisicaI = new Asignatura { Nombre = "Física I", Nivel = Nivel.Primero, Frecuencia = Frecuencia.Anual, UdbId = udb.Id };
            var quimica = new Asignatura { Nombre = "Química", Nivel = Nivel.Segundo, Frecuencia = Frecuencia.Anual, UdbId = udb.Id };
            var c101 = new Curso { Carrera = Especialidad.IQ, Año = Nivel.Primero, NroComision = 101, Turno = Turno.Mañana };
            var c102 = new Curso { Carrera = Especialidad.IQ, Año = Nivel.Primero, NroComision = 102, Turno = Turno.Tarde };
            var c201 = new Curso { Carrera = Especialidad.ISI, Año = Nivel.Segundo, NroComision = 201, Turno = Turno.Noche };
            context.AddRange(fisicaI, quimica, c101, c102, c201);
            context.SaveChanges();

            var dni = 0;
            Designacion Designacion(string apellido, string nombre, Estado estado = Estado.Activa, DateTime? fechaFin = null,
                params CargoDesignacion[] cargos) => new()
            {
                Docente = new Docente { Apellido = apellido, Nombre = nombre, Dni = (++dni).ToString() },
                FechaInicio = DateTime.Today.AddYears(-1),
                FechaFin = fechaFin,
                EstadoDesignacion = estado,
                Cargos = cargos.ToList()
            };
            CargoDesignacion Cargo(CargoDocente cargo, Condicion condicion = Condicion.Regular, bool enLicencia = false,
                params Actividad[] actividades) => new()
            {
                Cargo = cargo,
                Condicion = condicion,
                TipoDedicacion = TipoDedicacion.Simple,
                CantidadDedicacion = 1,
                UdbId = udb.Id,
                EnLicencia = enLicencia,
                Actividades = actividades.ToList()
            };
            Actividad Docencia(Asignatura asignatura, Curso curso, string? horario = null) =>
                new() { Especificacion = EspecificacionCargo.Docencia, Asignatura = asignatura, Curso = curso, Horario = horario };

            var cargoMendez = Cargo(CargoDocente.ProfesorTitular, enLicencia: true, actividades: Docencia(fisicaI, c101));

            context.Designaciones.AddRange(
                Designacion("Perez", "Juan", cargos: Cargo(CargoDocente.JefeTrabajosPracticos, actividades: new[]
                {
                    Docencia(fisicaI, c101, "LU 8 a 10"),
                    new Actividad { Especificacion = EspecificacionCargo.Gestion },
                    new Actividad { Especificacion = EspecificacionCargo.Investigacion, Asignatura = fisicaI },
                    new Actividad { Especificacion = EspecificacionCargo.Otro, Asignatura = fisicaI }
                })),
                Designacion("Mendez", "Ana", cargos: cargoMendez),
                Designacion("Gomez", "Luis", Estado.Finalizada, cargos: Cargo(CargoDocente.ProfesorAdjunto, actividades: Docencia(fisicaI, c101))),
                Designacion("Lopez", "Eva", fechaFin: DateTime.Today.AddDays(-1),
                    cargos: Cargo(CargoDocente.ProfesorAdjunto, actividades: Docencia(fisicaI, c101))),
                Designacion("Diaz", "Rosa", fechaFin: DateTime.Today.AddMonths(6),
                    cargos: Cargo(CargoDocente.ProfesorAdjunto, actividades: Docencia(fisicaI, c102))),
                Designacion("Ruiz", "Pablo", cargos: new[]
                {
                    Cargo(CargoDocente.ProfesorTitular, actividades: new Actividad { Especificacion = EspecificacionCargo.JefeDeCatedra, Asignatura = fisicaI }),
                    Cargo(CargoDocente.JefeTrabajosPracticos, actividades: new Actividad { Especificacion = EspecificacionCargo.Laboratorio, Asignatura = fisicaI })
                }),
                Designacion("Sosa", "Ines", cargos: Cargo(CargoDocente.ProfesorAdjunto, actividades: Docencia(quimica, c201))));
            context.SaveChanges();

            // Suplente de Mendez (necesita el Id del cargo en licencia)
            var suplente = Cargo(CargoDocente.ProfesorTitular, Condicion.Suplente, actividades: Docencia(fisicaI, c101));
            suplente.ReemplazaACargoId = cargoMendez.Id;
            context.Designaciones.Add(Designacion("Ibarra", "Raul", cargos: suplente));
            context.SaveChanges();

            _fisicaI = fisicaI.Id;
            _quimica = quimica.Id;
            _curso101 = c101.Id;
            _curso102 = c102.Id;
        }

        public void Dispose() => _db.Dispose();

        private DocentesPorCursoService CrearServicio(DocentesDbContext context) =>
            new(new DocentesPorCursoRepository(context), new AsignaturaRepository(context));

        private async Task<List<CursoDocentesDto>> PorCursoAsync(
            Especialidad? carrera = null, Nivel? año = null, int? cursoId = null)
        {
            using var context = _db.CrearContexto();
            return (await CrearServicio(context).GetPorCursoAsync(carrera, año, cursoId)).ToList();
        }

        [Fact]
        public async Task PorCurso_SoloCargosVigentes_OrdenadosPorJerarquia()
        {
            var cursos = await PorCursoAsync();

            var fisica101 = cursos.Single(c => c.CursoId == _curso101).Asignaturas.Single();
            fisica101.Nombre.Should().Be("Física I");
            // Gomez (finalizada) y Lopez (FechaFin vencida) no aparecen; PT antes que JTP
            fisica101.Docentes.Select(d => d.Apellido).Should().Equal("Ibarra", "Mendez", "Perez");
            fisica101.Docentes.Single(d => d.Apellido == "Perez").Horario.Should().Be("LU 8 a 10");

            // Diaz tiene FechaFin futura: vigente
            cursos.Single(c => c.CursoId == _curso102).Asignaturas.Single().Docentes
                .Should().ContainSingle().Which.Apellido.Should().Be("Diaz");
        }

        [Fact]
        public async Task PorCurso_ActividadesSinCurso_NoSonFilasDeDocentes()
        {
            var cursos = await PorCursoAsync();

            var docentes = cursos.SelectMany(c => c.Asignaturas).SelectMany(a => a.Docentes).ToList();
            docentes.Should().OnlyContain(d => d.Especificacion == EspecificacionCargo.Docencia);
            docentes.Count(d => d.Apellido == "Perez").Should().Be(1);
            docentes.Should().NotContain(d => d.Apellido == "Ruiz");
        }

        [Theory]
        [InlineData(null, null, false, new[] { 101, 102, 201 })]
        [InlineData(Especialidad.IQ, null, false, new[] { 101, 102 })]
        [InlineData(null, Nivel.Segundo, false, new[] { 201 })]
        [InlineData(Especialidad.IQ, Nivel.Primero, true, new[] { 102 })]
        [InlineData(Especialidad.ISI, Nivel.Primero, false, new int[0])]
        public async Task PorCurso_Filtros_SeCombinan(Especialidad? carrera, Nivel? año, bool soloCurso102, int[] comisiones)
        {
            var cursos = await PorCursoAsync(carrera, año, soloCurso102 ? _curso102 : null);

            cursos.Select(c => c.NroComision).Should().Equal(comisiones);
        }

        [Fact]
        public async Task PorCurso_MarcaLicenciaYSuplenteConElCargoQueReemplaza()
        {
            var docentes = (await PorCursoAsync(cursoId: _curso101)).Single().Asignaturas.Single().Docentes;

            var mendez = docentes.Single(d => d.Apellido == "Mendez");
            mendez.EnLicencia.Should().BeTrue();
            mendez.ReemplazaA.Should().BeNull();

            var ibarra = docentes.Single(d => d.Apellido == "Ibarra");
            ibarra.EsSuplente.Should().BeTrue();
            ibarra.ReemplazaA.Should().Be("Mendez, Ana – PT 1 DS");
        }

        [Fact]
        public async Task PorCurso_CatedraEnCadaCursoDeLaAsignatura()
        {
            var cursos = await PorCursoAsync();

            var fisica = cursos.SelectMany(c => c.Asignaturas).Where(a => a.AsignaturaId == _fisicaI).ToList();
            fisica.Should().HaveCount(2);
            fisica.Should().AllSatisfy(a => a.Catedra.Select(d => d.EspecificacionTexto)
                .Should().Equal("Jefe de Cátedra", "Laboratorio")); // PT antes que JTP
            cursos.SelectMany(c => c.Asignaturas).Single(a => a.AsignaturaId == _quimica).Catedra.Should().BeEmpty();
        }

        [Fact]
        public async Task PorAsignatura_TodosSusCursosYSuCatedra()
        {
            using var context = _db.CrearContexto();

            var reporte = await CrearServicio(context).GetPorAsignaturaAsync(_fisicaI);

            reporte.Nombre.Should().Be("Física I");
            reporte.NombreUdb.Should().Be("UDB Física");
            reporte.Cursos.Select(c => c.CursoId).Should().Equal(_curso101, _curso102);
            reporte.Cursos[0].Docentes.Select(d => d.Apellido).Should().Equal("Ibarra", "Mendez", "Perez");
            reporte.Catedra.Select(d => d.Apellido).Should().Equal("Ruiz", "Ruiz");
        }

        [Fact]
        public async Task PorAsignatura_Inexistente_NotFound()
        {
            using var context = _db.CrearContexto();

            var act = () => CrearServicio(context).GetPorAsignaturaAsync(99999);

            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("No se encontró la asignatura con ID 99999.");
        }
    }
}
