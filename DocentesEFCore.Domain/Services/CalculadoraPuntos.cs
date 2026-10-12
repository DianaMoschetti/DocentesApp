using DocentesApp.Domain.Enums;
using DocentesApp.Domain.Extensions;

namespace DocentesApp.Domain.Services
{
    // Reglas de puntos de un CargoDesignacion. Funciones puras: las usa el backend para calcular y guardar
    // los puntos, y la UI solo para mostrar una vista previa (el valor real lo guarda el backend).
    public static class CalculadoraPuntos
    {
        /// <summary>
        /// Puntos = PuntosBase x multiplicador de dedicación x CantidadDedicacion; 0 si el cargo está en licencia.
        /// Ej: Profesor Asociado DTP 1 = 157 x 2 x 1 = 314; Ayudante de 1° DS 0,5 = 100 x 1 x 0,5 = 50.
        /// </summary>
        public static decimal Calcular(decimal puntosBase, TipoDedicacion dedicacion, decimal cantidadDedicacion, bool enLicencia)
        {
            if (enLicencia)
                return 0;

            return puntosBase * dedicacion.Multiplicador() * cantidadDedicacion;
        }

        /// <summary>
        /// El Ayudante de 2° solo puede tener dedicación Simple.
        /// </summary>
        public static bool EsDedicacionValida(CargoDocente cargo, TipoDedicacion dedicacion)
        {
            if (cargo == CargoDocente.AyudanteSegunda)
                return dedicacion == TipoDedicacion.Simple;

            return true;
        }
    }
}
