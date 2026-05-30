namespace DocentesApp.Application.DTOs.Snapshots
{
    public class CreatePlantaSnapshotDto
    {
        public string Fuente { get; set; } = string.Empty; // Ej: "Importación Excel", "API externa", etc.
        public bool IsCurrent { get; set; }

        public List<CreatePlantaDocenteSnapshotDto> Filas { get; set; } = new();
       
    }
}
