using DocentesApp.API.Services;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared;
using AwesomeAssertions;

namespace DocentesApp.Tests.Auditoria
{
    // Piezas sin base de datos de la lectura del historial: zona horaria y formato de valores
    public class LecturaAuditoriaTests
    {
        [Fact]
        public void ZonaHoraria_SinElIdIana_UsaUtcMenos3Fijo()
        {
            var zona = ZonaHorariaArgentina.Crear("No/Existe");

            zona.BaseUtcOffset.Should().Be(TimeSpan.FromHours(-3));
            zona.SupportsDaylightSavingTime.Should().BeFalse();
        }

        [Fact]
        public void ZonaHoraria_InicioDelDiaDeArgentina_EsLas3UtcDelMismoDia()
        {
            ZonaHorariaArgentina.InicioDelDiaUtc(new DateOnly(2026, 3, 10)).Should().Be(new DateTime(2026, 3, 10, 3, 0, 0));
            ZonaHorariaArgentina.DesdeUtc(new DateTime(2026, 3, 10, 2, 30, 0)).Should().Be(new DateTime(2026, 3, 9, 23, 30, 0));
        }

        [Theory]
        [InlineData(typeof(EspecificacionCargo), "5", "Jefe de Cátedra")]   // [Description]
        [InlineData(typeof(RolUdb), "1", "Director")]                       // sin [Description]: el nombre
        [InlineData(typeof(RolUdb), "99", "99")]                            // valor que ya no existe: crudo
        [InlineData(typeof(bool), "true", "Sí")]
        [InlineData(typeof(bool), "false", "No")]
        [InlineData(typeof(DateTime), "2025-03-01T00:00:00", "01/03/2025")]
        [InlineData(typeof(DateTime?), "2025-03-01T10:15:00", "01/03/2025 10:15")]
        [InlineData(typeof(DateOnly?), "1980-07-21", "21/07/1980")]
        [InlineData(typeof(decimal), "1.5", "1,5")]
        [InlineData(typeof(string), "texto", "texto")]
        [InlineData(null, "crudo", "crudo")]                                // propiedad desconocida
        public void Formatear_SegunElTipoDeLaPropiedad(Type? tipo, string valor, string esperado)
        {
            AuditoriaService.Formatear(tipo, valor).Should().Be(esperado);
        }

        [Theory]
        [InlineData(typeof(DateTimeOffset), "0001-01-01T00:00:00.0000000+00:00")]
        [InlineData(typeof(DateTime), "0001-01-01T00:00:00")]
        public void Formatear_Fecha0001_NoSeMuestraComoFecha(Type tipo, string valor)
        {
            AuditoriaService.Formatear(tipo, valor).Should().BeNull();
        }
    }
}
