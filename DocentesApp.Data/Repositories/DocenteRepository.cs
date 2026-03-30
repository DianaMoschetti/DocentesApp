using DocentesApp.Data.Context;
using DocentesApp.Model;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class DocenteRepository
    {
        private readonly DocentesDbContext _context;

        public DocenteRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Docente>> GetAllDocentesAsync()
        {
            return await _context.Docentes.ToListAsync();
        }

        public async Task<Docente?> GetDocenteByIdAsync(int id)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Docente?> GetDocenteByIdConDesignacionesAsync(int id)
        {
            return await _context.Docentes
                .Include(d => d.Designaciones)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Docente?> GetDocenteByLegajoAsync(int legajo)
        {
            return await _context.Docentes.FirstOrDefaultAsync(d => d.Legajo == legajo);
        }

        public async Task<bool> ExisteDocenteByLegajoAsync(int legajo)
        {
            return await _context.Docentes.AnyAsync(d => d.Legajo == legajo);
        }

        public async Task AddDocenteAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
        }

        public void UpdateDocente(Docente docente)
        {
            _context.Docentes.Update(docente);
        }

        public void DeleteDocente(Docente docente)
        {
            _context.Docentes.Remove(docente);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
