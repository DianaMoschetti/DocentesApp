using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IPlantaReporteService
    {
        Task<PlantaUdbReporteDto> GetPlantaUdbPorAsignaturaAsync(int udbId);
        Task<PlantaUdbReporteDto> GetPlantaUdbPorDocenteAsync(int udbId);
        // Un reporte por cada UDB donde el docente tiene cargos vigentes
        Task<IEnumerable<PlantaUdbReporteDto>> GetPlantaByDocenteAsync(int docenteId);
    }
}