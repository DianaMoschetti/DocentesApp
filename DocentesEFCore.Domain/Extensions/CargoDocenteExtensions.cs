using DocentesApp.Domain.Enums;

namespace DocentesApp.Domain.Extensions
{
    public static class CargoDocenteExtensions
    {
        /// <summary>
        /// JTP y Ayudantes forman la categoría "Auxiliares" (para agrupar Profesores vs Auxiliares en reportes).
        /// </summary>
        public static bool EsAuxiliar(this CargoDocente cargo) => cargo switch
        {
            CargoDocente.JefeTrabajosPracticos => true,
            CargoDocente.AyudantePrimera => true,
            CargoDocente.AyudanteSegunda => true,
            _ => false
        };

        public static bool EsProfesor(this CargoDocente cargo) => cargo switch
        {
            CargoDocente.ProfesorAdjunto => true,
            CargoDocente.ProfesorAsociado => true,
            CargoDocente.ProfesorTitular => true,
            _ => false
        };

        /// <summary>
        /// Nombre corto para reportes: PAd, PAs, PT, JTP, Ay 1º, Ay 2º.
        /// </summary>
        public static string NombreCorto(this CargoDocente cargo) => cargo switch
        {
            CargoDocente.ProfesorAdjunto => "PAd",
            CargoDocente.ProfesorAsociado => "PAs",
            CargoDocente.ProfesorTitular => "PT",
            CargoDocente.JefeTrabajosPracticos => "JTP",
            CargoDocente.AyudantePrimera => "Ay 1º",
            CargoDocente.AyudanteSegunda => "Ay 2º",
            _ => cargo.ToString()
        };
    }
}
