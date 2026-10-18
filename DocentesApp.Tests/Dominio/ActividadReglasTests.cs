using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Services;
using AwesomeAssertions;

namespace DocentesApp.Tests.Dominio
{
    public class ActividadReglasTests
    {
        // Las 6 reglas: especificación -> asignatura / curso
        [Theory]
        [InlineData(EspecificacionCargo.Docencia, Requisito.Obligatorio, Requisito.Obligatorio)]
        [InlineData(EspecificacionCargo.Laboratorio, Requisito.Obligatorio, Requisito.Vacio)]
        [InlineData(EspecificacionCargo.JefeDeCatedra, Requisito.Opcional, Requisito.Vacio)]
        [InlineData(EspecificacionCargo.Investigacion, Requisito.Opcional, Requisito.Vacio)]
        [InlineData(EspecificacionCargo.Gestion, Requisito.Vacio, Requisito.Vacio)]
        [InlineData(EspecificacionCargo.Otro, Requisito.Opcional, Requisito.Opcional)]
        public void Reglas_PorEspecificacion(EspecificacionCargo especificacion, Requisito asignatura, Requisito curso)
        {
            ActividadReglas.Asignatura(especificacion).Should().Be(asignatura);
            ActividadReglas.Curso(especificacion).Should().Be(curso);
        }

        [Theory]
        [InlineData(Requisito.Obligatorio, 5, true)]
        [InlineData(Requisito.Obligatorio, null, false)]
        [InlineData(Requisito.Vacio, null, true)]
        [InlineData(Requisito.Vacio, 5, false)]
        [InlineData(Requisito.Opcional, null, true)]
        [InlineData(Requisito.Opcional, 5, true)]
        public void Cumple(Requisito requisito, int? valor, bool esperado)
        {
            ActividadReglas.Cumple(requisito, valor).Should().Be(esperado);
        }
    }
}
