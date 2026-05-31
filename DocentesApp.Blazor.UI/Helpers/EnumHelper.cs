using System.ComponentModel;
using System.Reflection;

namespace DocentesApp.Blazor.UI.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Devuelve el texto a mostrar para un valor de enum.
        /// Si el enum tiene [Description("texto legible")], usa ese.
        /// Si no, usa el nombre del valor tal cual está definido.
        /// </summary>
        public static string GetDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// Devuelve el texto a mostrar para un valor de enum
        /// Útil cuando tengo el valor como T en lugar de Enum
        /// </summary>
        public static string GetDisplay<T>(T value) where T : Enum
        {
            return GetDisplayName(value);
        }

        /// <summary>
        /// Devuelve todos los valores de un enum como lista de (valor, texto).
        /// Útil para construir dropdowns.
        /// </summary>
        public static List<(T Value, string Display)> GetOptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => (e, GetDisplayName(e)))
                .ToList();
        }
    }
}