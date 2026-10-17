namespace DocentesApp.Shared.Auth
{
    /// <summary>
    /// Nombres de los roles de Identity. Cada usuario tiene un solo rol.
    /// Los permisos no se chequean por rol sino por política (ver <see cref="Politicas"/>).
    /// </summary>
    public static class Roles
    {
        public const string Administrador = "Administrador"; // todo, incluida la gestión de usuarios
        public const string Editor = "Editor";               // todo menos la gestión de usuarios
        public const string Consultante = "Consultante";     // solo lectura (GETs y reportes)

        public static readonly string[] Todos = { Administrador, Editor, Consultante };

        public static bool EsValido(string? rol) => rol != null && Todos.Contains(rol);
    }
}
