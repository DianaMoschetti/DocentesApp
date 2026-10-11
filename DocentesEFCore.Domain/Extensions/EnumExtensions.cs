using System.Reflection;

namespace DocentesApp.Domain.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// true si el valor del enum está marcado con [Obsolete] (no se debe ofrecer en la UI ni aceptar en altas/ediciones).
        /// </summary>
        public static bool EsObsoleto(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<ObsoleteAttribute>() != null;
        }

        /// <summary>
        /// Texto a mostrar: el [Description] del valor si lo tiene, si no el nombre del valor.
        /// </summary>
        public static string Descripcion(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return field?.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description ?? value.ToString();
        }

        /// <summary>
        /// true si el valor está definido en el enum y no es obsoleto.
        /// </summary>
        public static bool EsValorVigente<T>(this T value) where T : struct, Enum
        {
            return Enum.IsDefined(value) && !value.EsObsoleto();
        }
    }
}
