namespace DocentesApp.Shared.DTOs.Reportes
{
    public class PlantaUdbReporteDto
    {
        public int UdbId { get; set; }
        public string NombreUdb { get; set; } = string.Empty;
        public List<PlantaUdbFilaDto> Filas { get; set; } = new();
        // Suma de los puntos de los cargos de la UDB (cada cargo una sola vez)
        public decimal TotalPuntos { get; set; }
    }
}
