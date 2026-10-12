using DocentesApp.Domain.Services;
using DocentesApp.Shared.DTOs.Designaciones;
using DocentesApp.Shared.DTOs.PuntosPorCargo;

namespace DocentesApp.Blazor.UI.Helpers
{
    public static class PuntosPreviewHelper
    {
        /// <summary>
        /// Vista previa de los puntos de un cargo en el formulario, con la misma fórmula que el backend
        /// (CalculadoraPuntos) y el catálogo cargado. El valor que se guarda lo calcula el backend.
        /// null si el cargo no tiene fila en Puntos por cargo.
        /// </summary>
        public static decimal? Calcular(UpsertCargoDesignacionDto cargo, IEnumerable<PuntosPorCargoDto> catalogo)
        {
            var fila = catalogo.FirstOrDefault(p => p.Cargo == cargo.Cargo);
            if (fila == null)
                return null;

            return CalculadoraPuntos.Calcular(fila.PuntosBase, cargo.TipoDedicacion, cargo.CantidadDedicacion, cargo.EnLicencia);
        }
    }
}
