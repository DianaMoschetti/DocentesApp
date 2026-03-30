using DocentesApp.Data.Context;
using DocentesApp.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using DocentesApp.Domain.Entities;

namespace DocentesApp.Data.Repositories
{
    public class DocenteRepository : IDocenteRepository
    {
        private readonly DocentesDbContext _context;

        public DocenteRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Docente>> GetAllAsync()
        {
            return await _context.Docentes
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Docente?> GetByIdAsync(int id)
        {
            return await _context.Docentes
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Docente?> GetByLegajoAsync(int legajo)
        {
            return await _context.Docentes
                .FirstOrDefaultAsync(d => d.Legajo == legajo);
        }

        public async Task<bool> ExistsByLegajoAsync(int legajo)
        {
            return await _context.Docentes
                .AnyAsync(d => d.Legajo == legajo);
        }

        public async Task AddAsync(Docente docente)
        {
            await _context.Docentes.AddAsync(docente);
        }

        public void Update(Docente docente)
        {
            _context.Docentes.Update(docente);
        }

        public void Delete(Docente docente)
        {
            _context.Docentes.Remove(docente);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasDesignacionesAsync(int docenteId)
        {
            return await _context.Designaciones
                .AnyAsync(d => d.DocenteId == docenteId);
        }
    }
}
