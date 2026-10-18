using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDocenteRepository
    {
        Task<IEnumerable<Docente>> GetAllAsync();
        Task<Docente?> GetByIdAsync(int id);
        // el legajo es opcional (v5): sin legajo no hay docente ni duplicado
        Task<Docente?> GetByLegajoAsync(int? legajo);
        Task<bool> ExistsByLegajoAsync(int? legajo);
        Task<bool> ExistsByDniAsync(string dni);
        Task<bool> ExistsAnotherByDniAsync(string dni, int docenteId); // Para validar que no exista otro docente con el mismo DNI al actualizar un docente existente
        Task AddAsync(Docente docente);
        void Update(Docente docente);
        void Delete(Docente docente);
        // Carga las membresías (DocenteUdbs) del docente trackeado, antes de borrarlo
        Task CargarMembresiasAsync(Docente docente);
        Task SaveChangesAsync();
        Task<bool> HasDesignacionesAsync(int docenteId);      

    }
}
