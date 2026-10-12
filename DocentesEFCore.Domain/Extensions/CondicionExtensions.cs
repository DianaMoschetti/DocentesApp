using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Extensions
{
    public static class CondicionExtensions
    {
        /// <summary>
        /// Nombre corto para reportes: Ord, R, Int, Sup, Otro.
        /// </summary>
        public static string NombreCorto(this Condicion condicion) => condicion switch
        {
            Condicion.Ordinario => "Ord",
            Condicion.Regular => "R",
            Condicion.Interino => "Int",
            Condicion.Suplente => "Sup",
            Condicion.Otros => "Otro",
            _ => condicion.ToString()
        };
    }
}
