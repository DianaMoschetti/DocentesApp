using DocentesApp.Application.Reportes;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDocentesPorCursoRepository
    {
        // Actividades con asignatura y curso de cargos vigentes (docencia y "otro" con curso). Los filtros se combinan.
        Task<IEnumerable<DocenteCursoRegistro>> GetConCursoAsync(Especialidad? carrera, Nivel? año, int? cursoId, int? asignaturaId);
        // Actividades de cátedra (Jefe de Cátedra y Laboratorio: asignatura sin curso) de cargos vigentes, de esas asignaturas
        Task<IEnumerable<DocenteCursoRegistro>> GetCatedraAsync(IEnumerable<int> asignaturaIds);
    }
}
