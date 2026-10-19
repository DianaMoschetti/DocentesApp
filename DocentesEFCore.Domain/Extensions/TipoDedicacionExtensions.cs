using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Extensions
{
    public static class TipoDedicacionExtensions
    {
        /// <summary>
        /// Multiplicador de puntos: Simple / DS (10 hs) x1, Semiexclusiva / DTP (20 hs) x2, Exclusiva / DE (40 hs) x4.
        /// Un valor no definido devuelve 0 para no sumar puntos por error. La media dedicación se carga con CantidadDedicacion = 0,5.
        /// </summary>
        public static int Multiplicador(this TipoDedicacion dedicacion) => dedicacion switch
        {
            TipoDedicacion.Simple => 1,
            TipoDedicacion.SemiExclusiva => 2,
            TipoDedicacion.Exclusiva => 4,
            _ => 0
        };

        /// <summary>
        /// Nombre corto para reportes: DS, DTP, DE.
        /// </summary>
        public static string NombreCorto(this TipoDedicacion dedicacion) => dedicacion switch
        {
            TipoDedicacion.Simple => "DS",
            TipoDedicacion.SemiExclusiva => "DTP",
            TipoDedicacion.Exclusiva => "DE",
            _ => dedicacion.ToString()
        };
    }
}
