using DocentesApp.Data.Context;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class DedicacionRepository : IDedicacionRepository
    {
        private readonly DocentesDbContext _context;

        public DedicacionRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dedicacion>> GetAllAsync()
        {
            return await _context.Dedicaciones
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Dedicacion?> GetByIdAsync(int id)
        {
            return await _context.Dedicaciones
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(Dedicacion dedicacion)
        {
            await _context.Dedicaciones.AddAsync(dedicacion);
        }

        public void Delete(Dedicacion dedicacion)
        {
            _context.Dedicaciones.Remove(dedicacion);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasDesignacionesAsync(int dedicacionId)
        {
            return await _context.Designaciones
                .AnyAsync(d => d.DedicacionId == dedicacionId);
        }
    }
}