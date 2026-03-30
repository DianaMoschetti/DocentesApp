using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDocenteRepository
    {
        Task<IEnumerable<Docente>> GetAllAsync();
        Task<Docente?> GetByIdAsync(int id);
        Task<Docente?> GetByLegajoAsync(int legajo);
        Task<bool> ExistsByLegajoAsync(int legajo);
        Task AddAsync(Docente docente);
        void Update(Docente docente);
        void Delete(Docente docente);
        Task SaveChangesAsync();
        Task<bool> HasDesignacionesAsync(int docenteId);
        //Task<List<Docente>> GetAllDocentesAsync();
        //Task<Docente?> GetDocenteByIdAsync(int id);
        //Task<Docente?> GetDocenteByIdConDesignacionesAsync(int id);
        //Task<Docente?> GetDocenteByLegajoAsync(int legajo);
        //Task<bool> ExisteDocenteByLegajoAsync(int legajo);
        //Task AddDocenteAsync(Docente docente);
        //void UpdateDocente(Docente docente);
        //void DeleteDocente(Docente docente);
        //Task SaveChangesAsync();

    }
}
