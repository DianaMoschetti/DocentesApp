using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Services;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;

namespace DocentesApp.Tests.Dominio
{
    public class CalculadoraPuntosTests
    {
        // Puntos base del catálogo (HasData de PuntosPorCargo)
        private const int ProfesorAdjunto = 138;
        private const int ProfesorAsociado = 157;
        private const int ProfesorTitular = 176;
        private const int AyudantePrimera = 100;

        [Theory]
        [InlineData(ProfesorAsociado, TipoDedicacion.SemiExclusiva, 1, 314)] // 157 x 2 x 1
        [InlineData(ProfesorAdjunto, TipoDedicacion.Exclusiva, 1, 552)]      // 138 x 4 x 1
        [InlineData(ProfesorTitular, TipoDedicacion.Simple, 1.5, 264)]       // 176 x 1 x 1,5
        [InlineData(AyudantePrimera, TipoDedicacion.Simple, 0.5, 50)]        // 100 x 1 x 0,5
        public void Calcular_SinLicencia_MultiplicaBasePorDedicacionYCantidad(
            int puntosBase, TipoDedicacion dedicacion, double cantidad, int esperado)
        {
            // los atributos no admiten decimal: se convierten acá
            var puntos = CalculadoraPuntos.Calcular(puntosBase, dedicacion, (decimal)cantidad, enLicencia: false);

            puntos.Should().Be(esperado);
        }

        [Fact]
        public void Calcular_EnLicencia_DevuelveCero()
        {
            var puntos = CalculadoraPuntos.Calcular(ProfesorAsociado, TipoDedicacion.SemiExclusiva, 1, enLicencia: true);

            puntos.Should().Be(0);
        }

        [Theory]
        [InlineData(TipoDedicacion.SemiExclusiva)]
        [InlineData(TipoDedicacion.Exclusiva)]
        public void EsDedicacionValida_AyudanteSegundaConDtpODe_EsFalso(TipoDedicacion dedicacion)
        {
            CalculadoraPuntos.EsDedicacionValida(CargoDocente.AyudanteSegunda, dedicacion).Should().BeFalse();
        }

        [Fact]
        public void EsDedicacionValida_AyudanteSegundaSimple_EsVerdadero()
        {
            CalculadoraPuntos.EsDedicacionValida(CargoDocente.AyudanteSegunda, TipoDedicacion.Simple).Should().BeTrue();
        }

        [Theory]
        [InlineData(CargoDocente.ProfesorAdjunto)]
        [InlineData(CargoDocente.JefeTrabajosPracticos)]
        [InlineData(CargoDocente.AyudantePrimera)]
        public void EsDedicacionValida_OtrosCargosConExclusiva_EsVerdadero(CargoDocente cargo)
        {
            CalculadoraPuntos.EsDedicacionValida(cargo, TipoDedicacion.Exclusiva).Should().BeTrue();
        }
    }
}
