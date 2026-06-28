using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IAsignaturaRepository
    {
        Task<IEnumerable<Asignatura>> GetAllAsync();
        Task<Asignatura?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(Materia nombre, Nivel nivel, int? udbId, int? excludeId = null);
        Task AddAsync(Asignatura asignatura);
        void Update(Asignatura asignatura);
        void Delete(Asignatura asignatura);
        Task SaveChangesAsync();
        Task<bool> HasDetallesDesignacionAsync(int asignaturaId);
    }
}
