using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IPlantaReporteService
    {
        // [Diana desde v4.0 OBSOLETO] reemplazados por el reporte v5 (PlantaUdbReporteDto)
        //Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId);
        //Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId);

        Task<PlantaUdbReporteDto> GetPlantaUdbPorAsignaturaAsync(int udbId);
        Task<PlantaUdbReporteDto> GetPlantaUdbPorDocenteAsync(int udbId);
        // Un reporte por cada UDB donde el docente tiene cargos vigentes
        Task<IEnumerable<PlantaUdbReporteDto>> GetPlantaByDocenteAsync(int docenteId);
    }
}