using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IUdbRepository
    {
        Task<IEnumerable<Udb>> GetAllAsync();
        Task<Udb?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(string nombre, int? excludeId = null);
        Task AddAsync(Udb udb);
        void Update(Udb udb);
        void Delete(Udb udb);
        Task SaveChangesAsync();
        Task<bool> HasAsignaturasAsync(int udbId);
    }
}
