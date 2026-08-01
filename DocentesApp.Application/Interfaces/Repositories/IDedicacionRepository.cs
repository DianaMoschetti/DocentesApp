using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    // [Diana desde v4.0 OBSOLETO]
    [Obsolete("IDedicacionRepository obsoleto desde v4.0.")]
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