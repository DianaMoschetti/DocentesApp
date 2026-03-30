using DocentesApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDocenteRepository
    {
        Task<List<Docente>> GetAllDocentesAsync();
        Task<Docente?> GetDocenteByIdAsync(int id);
        Task<Docente?> GetDocenteByIdConDesignacionesAsync(int id);
        Task<Docente?> GetDocenteByLegajoAsync(int legajo);
        Task<bool> ExisteDocenteByLegajoAsync(int legajo);
        Task AddDocenteAsync(Docente docente);
        void UpdateDocente(Docente docente);
        void DeleteDocente(Docente docente);
        Task SaveChangesAsync();

    }
}
