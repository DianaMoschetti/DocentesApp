using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IUdbRepository
    {
        Task<IEnumerable<Udb>> GetAllAsync();
        Task<Udb?> GetByIdAsync(int id);
        // Para editar: trackeada y sin navegaciones (Director/Secretario no se cargan ni se tocan)
        Task<Udb?> GetByIdParaEditarAsync(int id);
        // Para borrar: con sus membresías, para que el borrado en cascada quede trackeado (y auditado)
        Task<Udb?> GetByIdConMembresiasAsync(int id);
        Task<bool> ExisteDocenteAsync(int docenteId);
        Task<bool> ExistsAsync(string nombre, int? excludeId = null);
        Task AddAsync(Udb udb);
        void Update(Udb udb);
        void Delete(Udb udb);
        Task SaveChangesAsync();
        Task<bool> HasAsignaturasAsync(int udbId);
        // Membresía docente-UDB (v5): al crear un cargo en una UDB a la que el docente no pertenece, se agrega
        Task<bool> EsMiembroAsync(int docenteId, int udbId);
        Task AddMiembroAsync(DocenteUdb docenteUdb);
    }
}
