using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IPlantaReporteService
    {
        Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId);
        Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId);
    }
}