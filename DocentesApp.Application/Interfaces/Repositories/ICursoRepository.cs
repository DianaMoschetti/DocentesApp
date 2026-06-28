using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface ICursoRepository
    {
        Task<IEnumerable<Curso>> GetAllAsync();
        Task<Curso?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(Turno turno, Nivel año, Especialidad carrera, int nroComision, int? excludeId = null);
        Task AddAsync(Curso curso);
        void Update(Curso curso);
        void Delete(Curso curso);
        Task SaveChangesAsync();
        Task<bool> HasDependenciasAsync(int cursoId);
    }
}
