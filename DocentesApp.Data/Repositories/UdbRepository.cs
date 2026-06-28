using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class UdbRepository : IUdbRepository
    {
        private readonly DocentesDbContext _context;

        public UdbRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Udb>> GetAllAsync()
        {
            return await _context.Udbs
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Udb?> GetByIdAsync(int id)
        {
            return await _context.Udbs
                .Include(u => u.Director)
                .Include(u => u.Secretario)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsAsync(string nombre, int? excludeId = null)
        {
            return await _context.Udbs
                .AnyAsync(u => u.Nombre == nombre && (excludeId == null || u.Id != excludeId));
        }

        public async Task AddAsync(Udb udb)
        {
            await _context.Udbs.AddAsync(udb);
        }

        public void Update(Udb udb)
        {
            _context.Udbs.Update(udb);
        }

        public void Delete(Udb udb)
        {
            _context.Udbs.Remove(udb);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasAsignaturasAsync(int udbId)
        {
            return await _context.Asignaturas
                .AnyAsync(a => a.UdbId == udbId);
        }
    }
}
