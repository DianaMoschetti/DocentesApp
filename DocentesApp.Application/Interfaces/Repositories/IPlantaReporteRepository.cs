using DocentesApp.Application.Reportes;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IPlantaReporteRepository
    {
        // [Diana desde v4.0 OBSOLETO] reemplazados por los registros crudos v5 (una fila por Actividad)
        //Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByUdbAsync(int udbId);
        //Task<IEnumerable<PlantaDocenteReporteDto>> GetPlantaByDocenteAsync(int docenteId);

        // Una fila por Actividad de cada CargoDesignacion vigente cuyo cargo pertenece a la UDB
        Task<IEnumerable<PlantaUdbRegistro>> GetRegistrosByUdbAsync(int udbId);
        // Una fila por Actividad de cada CargoDesignacion vigente del docente (de cualquier UDB)
        Task<IEnumerable<PlantaUdbRegistro>> GetRegistrosByDocenteAsync(int docenteId);
    }
}
