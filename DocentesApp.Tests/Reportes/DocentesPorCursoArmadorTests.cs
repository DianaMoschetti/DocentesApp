using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;
using AwesomeAssertions;

namespace DocentesApp.Tests.Reportes
{
    public class DocentesPorCursoArmadorTests
    {
        private const int FisicaI = 1;
        private const int Quimica = 2;

        private sealed record Curso(int Id, Especialidad Carrera, Nivel Año, int Comision, Turno Turno);

        private static readonly Curso Iq101 = new(10, Especialidad.IQ, Nivel.Primero, 101, Turno.Mañana);
        private static readonly Curso Iq102 = new(11, Especialidad.IQ, Nivel.Primero, 102, Turno.Tarde);
        private static readonly Curso Isi201 = new(12, Especialidad.ISI, Nivel.Segundo, 201, Turno.Noche);

        private static int _cargoId;

        // Fila de docencia (con curso) o de cátedra (curso null)
        private static DocenteCursoRegistro Registro(
            string apellido, CargoDocente cargo, int asignaturaId, Curso? curso,
            EspecificacionCargo especificacion = EspecificacionCargo.Docencia,
            Condicion condicion = Condicion.Regular, bool enLicencia = false) => new()
        {
            CargoDesignacionId = ++_cargoId,
            DocenteId = _cargoId,
            DocenteApellido = apellido,
            DocenteNombre = "N",
            Cargo = cargo,
            Condicion = condicion,
            EnLicencia = enLicencia,
            Especificacion = especificacion,
            AsignaturaId = asignaturaId,
            AsignaturaNombre = asignaturaId == FisicaI ? "Física I" : "Química",
            CursoId = curso?.Id,
            CursoAño = curso?.Año,
            CursoCarrera = curso?.Carrera,
            CursoNroComision = curso?.Comision,
            CursoTurno = curso?.Turno
        };

        private static DocenteCursoRegistro Catedra(string apellido, CargoDocente cargo, int asignaturaId,
            EspecificacionCargo especificacion = EspecificacionCargo.JefeDeCatedra) =>
            Registro(apellido, cargo, asignaturaId, null, especificacion);

        [Fact]
        public void PorCurso_AgrupaPorCursoYAsignatura_OrdenaCursosYAsignaturas()
        {
            var conCurso = new[]
            {
                Registro("Sosa", CargoDocente.ProfesorAdjunto, Quimica, Isi201),
                Registro("Perez", CargoDocente.ProfesorAdjunto, Quimica, Iq101),
                Registro("Gomez", CargoDocente.ProfesorAdjunto, FisicaI, Iq102),
                Registro("Lopez", CargoDocente.ProfesorAdjunto, FisicaI, Iq101),
            };

            var cursos = DocentesPorCursoArmador.PorCurso(conCurso, Array.Empty<DocenteCursoRegistro>());

            cursos.Select(c => c.NroComision).Should().Equal(101, 102, 201);
            cursos[0].Descripcion.Should().Be("Primero - IQ - Comisión 101 - Mañana");
            cursos[0].Asignaturas.Select(a => a.Nombre).Should().Equal("Física I", "Química");
            cursos[0].Asignaturas[0].Docentes.Should().ContainSingle().Which.Apellido.Should().Be("Lopez");
            cursos[0].Asignaturas[0].Catedra.Should().BeEmpty();
        }

        [Fact]
        public void Docentes_OrdenadosPorJerarquiaDelCargoYApellido_EnCursoCatedraYVistaPorAsignatura()
        {
            var conCurso = new[]
            {
                Registro("Alvarez", CargoDocente.AyudanteSegunda, FisicaI, Iq101),
                Registro("Zapata", CargoDocente.ProfesorAdjunto, FisicaI, Iq101),
                Registro("Moreno", CargoDocente.ProfesorTitular, FisicaI, Iq101),
                Registro("Bianchi", CargoDocente.JefeTrabajosPracticos, FisicaI, Iq101),
                Registro("Castro", CargoDocente.ProfesorAsociado, FisicaI, Iq101),
                Registro("Diaz", CargoDocente.AyudantePrimera, FisicaI, Iq101),
                Registro("Acosta", CargoDocente.ProfesorTitular, FisicaI, Iq101),
            };
            var catedra = new[]
            {
                Catedra("Ferreyra", CargoDocente.JefeTrabajosPracticos, FisicaI, EspecificacionCargo.Laboratorio),
                Catedra("Benitez", CargoDocente.ProfesorAdjunto, FisicaI),
                Catedra("Gimenez", CargoDocente.ProfesorTitular, FisicaI),
            };
            var esperado = new[] { "Acosta", "Moreno", "Castro", "Zapata", "Bianchi", "Diaz", "Alvarez" };
            var catedraEsperada = new[] { "Gimenez", "Benitez", "Ferreyra" };

            var porCurso = DocentesPorCursoArmador.PorCurso(conCurso, catedra).Single().Asignaturas.Single();
            var porAsignatura = DocentesPorCursoArmador.PorAsignatura(FisicaI, "Física I", "UDB Física", conCurso, catedra);

            porCurso.Docentes.Select(d => d.Apellido).Should().Equal(esperado);
            porCurso.Catedra.Select(d => d.Apellido).Should().Equal(catedraEsperada);
            porAsignatura.Cursos.Single().Docentes.Select(d => d.Apellido).Should().Equal(esperado);
            porAsignatura.Catedra.Select(d => d.Apellido).Should().Equal(catedraEsperada);
        }

        [Fact]
        public void PorCurso_CatedraSeMuestraEnCadaCursoDeLaAsignatura()
        {
            var conCurso = new[]
            {
                Registro("Perez", CargoDocente.ProfesorAdjunto, FisicaI, Iq101),
                Registro("Gomez", CargoDocente.ProfesorAdjunto, FisicaI, Iq102),
                Registro("Lopez", CargoDocente.ProfesorAdjunto, Quimica, Iq101),
            };
            var catedra = new[] { Catedra("Ruiz", CargoDocente.ProfesorTitular, FisicaI) };

            var cursos = DocentesPorCursoArmador.PorCurso(conCurso, catedra);

            var fisica = cursos.SelectMany(c => c.Asignaturas).Where(a => a.AsignaturaId == FisicaI).ToList();
            fisica.Should().HaveCount(2);
            fisica.Should().AllSatisfy(a => a.Catedra.Should().ContainSingle().Which.Apellido.Should().Be("Ruiz"));
            cursos[0].Asignaturas.Single(a => a.AsignaturaId == Quimica).Catedra.Should().BeEmpty();
        }

        [Fact]
        public void PorCurso_MarcaLicenciaYSuplenteConElCargoQueReemplaza()
        {
            var enLicencia = Registro("Mendez", CargoDocente.ProfesorTitular, FisicaI, Iq101, enLicencia: true);
            var suplente = Registro("Ibarra", CargoDocente.ProfesorTitular, FisicaI, Iq101, condicion: Condicion.Suplente);
            suplente.ReemplazaApellido = "Mendez";
            suplente.ReemplazaNombre = "Ana";
            suplente.ReemplazaCargo = CargoDocente.ProfesorTitular;
            suplente.ReemplazaCantidadDedicacion = 1;
            suplente.ReemplazaTipoDedicacion = TipoDedicacion.Simple;

            var docentes = DocentesPorCursoArmador.PorCurso(new[] { enLicencia, suplente }, Array.Empty<DocenteCursoRegistro>())
                .Single().Asignaturas.Single().Docentes;

            var mendez = docentes.Single(d => d.Apellido == "Mendez");
            mendez.EnLicencia.Should().BeTrue();
            mendez.EsSuplente.Should().BeFalse();
            mendez.ReemplazaA.Should().BeNull();

            var ibarra = docentes.Single(d => d.Apellido == "Ibarra");
            ibarra.EnLicencia.Should().BeFalse();
            ibarra.EsSuplente.Should().BeTrue();
            ibarra.ReemplazaA.Should().Be("Mendez, Ana – PT 1 DS");
            ibarra.CargoTexto.Should().Be("Profesor Titular");
            ibarra.CondicionTexto.Should().Be("Suplente");
            ibarra.EspecificacionTexto.Should().Be("Docencia");
        }

        [Fact]
        public void PorAsignatura_CursosOrdenadosConSusDocentes()
        {
            var conCurso = new[]
            {
                Registro("Sosa", CargoDocente.ProfesorAdjunto, FisicaI, Isi201),
                Registro("Perez", CargoDocente.ProfesorAdjunto, FisicaI, Iq101),
            };

            var reporte = DocentesPorCursoArmador.PorAsignatura(FisicaI, "Física I", "UDB Física", conCurso, Array.Empty<DocenteCursoRegistro>());

            reporte.Nombre.Should().Be("Física I");
            reporte.NombreUdb.Should().Be("UDB Física");
            reporte.Catedra.Should().BeEmpty();
            reporte.Cursos.Select(c => c.Descripcion).Should().Equal(
                "Primero - IQ - Comisión 101 - Mañana",
                "Segundo - ISI - Comisión 201 - Noche");
            reporte.Cursos[1].Docentes.Single().Apellido.Should().Be("Sosa");
        }
    }
}
