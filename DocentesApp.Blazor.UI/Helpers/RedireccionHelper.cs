namespace DocentesApp.Blazor.UI.Helpers
{
    public static class RedireccionHelper
    {
        /// <summary>
        /// Evita el open redirect: solo acepta rutas locales ("/algo"). Rechaza "//host", "/\host", URLs absolutas
        /// y caracteres de control (el navegador los descarta: "/\t/host" terminaría siendo "//host").
        /// </summary>
        public static bool EsUrlLocal(string? url) =>
            !string.IsNullOrEmpty(url) && url[0] == '/' &&
            (url.Length == 1 || (url[1] != '/' && url[1] != '\\')) &&
            !url.Any(char.IsControl);
    }
}
