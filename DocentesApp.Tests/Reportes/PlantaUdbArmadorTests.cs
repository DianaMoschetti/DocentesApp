using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;

namespace DocentesApp.Tests.Reportes
{
    public class PlantaUdbArmadorTests
    {
        // Cargo 1: Perez, PAs DTP 1 (314), docencia en Física II (2° nivel) y en Física I (1° nivel)
        // Cargo 2: Gomez, JTP DS 1 (119), gestión
        // Cargo 3: Lopez, Ay 1º DS 0,5 (50), investigación
        // Cargo 4: Diaz, PAd DS 1 (0, en licencia), otro sin asignatura
        private static List<PlantaUdbRegistro> Registros() => new()
        {
            Registro(1, "Perez", CargoDocente.ProfesorAsociado, 314, EspecificacionCargo.Docencia, "Física II", Nivel.Segundo),
            Registro(1, "Perez", CargoDocente.ProfesorAsociado, 314, EspecificacionCargo.Docencia, "Física I", Nivel.Primero),
            Registro(2, "Gomez", CargoDocente.JefeTrabajosPracticos, 119, EspecificacionCargo.Gestion),
            Registro(3, "Lopez", CargoDocente.AyudantePrimera, 50, EspecificacionCargo.Investigacion),
            Registro(4, "Diaz", CargoDocente.ProfesorAdjunto, 0, EspecificacionCargo.Otro, enLicencia: true),
        };

        [Fact]
        public void PorAsignatura_OrdenaSecciones_GestionLaboratoriosAsignaturasPorNivelYOtros()
        {
            var reporte = PlantaUdbArmador.PorAsignatura(3, "UDB Física", Registros());

            reporte.Filas.Select(f => f.Seccion).Should().Equal(
                PlantaUdbArmador.SeccionGestion,
                PlantaUdbArmador.SeccionLaboratoriosInvestigacion,
                "Física I",
                "Física II",
                PlantaUdbArmador.SeccionOtros);
        }

        [Fact]
        public void PorAsignatura_CargoEnLicencia_QuedaEnSuSeccionMarcado()
        {
            var reporte = PlantaUdbArmador.PorAsignatura(3, "UDB Física", Registros());

            var fila = reporte.Filas.Single(f => f.CargoDesignacionId == 4);
            fila.Seccion.Should().Be(PlantaUdbArmador.SeccionOtros);
            fila.EnLicencia.Should().BeTrue();
            fila.Observaciones.Should().Be("Licencia por estudio");
        }

        [Fact]
        public void PorDocente_PuntosDelCargoSoloEnSuPrimeraFila()
        {
            var reporte = PlantaUdbArmador.PorDocente(3, "UDB Física", Registros());

            var filasPerez = reporte.Filas.Where(f => f.CargoDesignacionId == 1).ToList();
            filasPerez.Should().HaveCount(2);
            filasPerez[0].Puntos.Should().Be(314);
            filasPerez[1].Puntos.Should().Be(0);

            reporte.Filas.Select(f => f.Docente).Should().BeInAscendingOrder();
        }

        [Fact]
        public void TotalPuntos_SumaCadaCargoUnaSolaVez()
        {
            var porAsignatura = PlantaUdbArmador.PorAsignatura(3, "UDB Física", Registros());
            var porDocente = PlantaUdbArmador.PorDocente(3, "UDB Física", Registros());

            // 314 + 119 + 50 + 0 (el de Perez no se cuenta dos veces)
            porAsignatura.TotalPuntos.Should().Be(483);
            porDocente.TotalPuntos.Should().Be(483);
            porDocente.Filas.Sum(f => f.Puntos).Should().Be(483);
        }

        [Fact]
        public void Fila_UsaNombresCortosYDedicacionConCantidad()
        {
            var reporte = PlantaUdbArmador.PorDocente(3, "UDB Física", Registros());

            var fila = reporte.Filas.Single(f => f.CargoDesignacionId == 3);
            fila.CargoCorto.Should().Be("Ay 1º");
            fila.CondicionCorta.Should().Be("Int");
            fila.Dedicacion.Should().Be("0,5 DS");
            fila.Legajo.Should().BeNull();
        }

        private static PlantaUdbRegistro Registro(
            int cargoId,
            string apellido,
            CargoDocente cargo,
            decimal puntos,
            EspecificacionCargo especificacion,
            string? asignatura = null,
            Nivel? nivel = null,
            bool enLicencia = false) => new()
        {
            CargoDesignacionId = cargoId,
            DocenteApellido = apellido,
            DocenteNombre = "N",
            Legajo = cargoId == 3 ? null : cargoId * 100,
            UdbId = 3,
            NombreUdb = "UDB Física",
            Cargo = cargo,
            Condicion = Condicion.Interino,
            TipoDedicacion = cargo == CargoDocente.ProfesorAsociado ? TipoDedicacion.SemiExclusiva : TipoDedicacion.Simple,
            CantidadDedicacion = cargo == CargoDocente.AyudantePrimera ? 0.5m : 1,
            Puntos = puntos,
            EnLicencia = enLicencia,
            CargoObservaciones = enLicencia ? "Licencia por estudio" : null,
            Especificacion = especificacion,
            Asignatura = asignatura,
            AsignaturaNivel = nivel,
            AsignaturaFrecuencia = asignatura == null ? null : Frecuencia.Anual
        };
    }
}
