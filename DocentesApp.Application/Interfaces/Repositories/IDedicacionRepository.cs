using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDedicacionRepository
    {
        Task<IEnumerable<Dedicacion>> GetAllAsync();
        Task<Dedicacion?> GetByIdAsync(int id);
        Task AddAsync(Dedicacion dedicacion);
        void Delete(Dedicacion dedicacion);
        Task SaveChangesAsync();
        Task<bool> HasDesignacionesAsync(int dedicacionId);
    }
}