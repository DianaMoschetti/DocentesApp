using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Reportes;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IDocentesPorCursoService
    {
        // Cursos con sus asignaturas y los docentes de cargos vigentes. Filtros opcionales (se combinan).
        Task<IEnumerable<CursoDocentesDto>> GetPorCursoAsync(Especialidad? carrera, Nivel? año, int? cursoId);
        // Una asignatura con todos sus cursos y docentes vigentes
        Task<AsignaturaDocentesDto> GetPorAsignaturaAsync(int asignaturaId);
    }
}
