namespace DocentesApp.Shared.DTOs.PuntosPorCargo
{
    // Solo se editan los puntos: el cargo es la clave de la fila.
    // Cambiar el valor no altera los puntos ya guardados en designaciones existentes.
    public class UpdatePuntosPorCargoDto
    {
        public decimal PuntosBase { get; set; }
    }
}
