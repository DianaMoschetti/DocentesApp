using DocentesApp.Data.Context;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace DocentesApp.Data.Repositories
{
    // [OBSOLETO v4.0] Dedicacion reemplazada por atributos en DetalleDesignacion
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
        // [Diana desde v4.0 OBSOLETO] DedicacionId ya no existe en Designacion
        // public async Task<bool> HasDesignacionesAsync(int dedicacionId)
        // {
        //     return await _context.Designaciones
        //         .AnyAsync(d => d.DedicacionId == dedicacionId);
        // }
        public async Task<bool> HasDesignacionesAsync(int dedicacionId)
        {
            // [Diana desde v4.0 OBSOLETO] DedicacionId ya no existe en Designacion — siempre devuelve false
            return false; // arreglar cuando actualice la interfaz
        }
    }
}