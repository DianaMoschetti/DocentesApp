using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IPlantaReporteRepository
    {
        Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId);
        Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId);
    }
}
