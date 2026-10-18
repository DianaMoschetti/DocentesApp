using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace DocentesApp.Data.Repositories
{
    public class AsignaturaRepository : IAsignaturaRepository
    {
        private readonly DocentesDbContext _context;
        public AsignaturaRepository(DocentesDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Asignatura>> GetAllAsync()
        {
            return await _context.Asignaturas
                .Include(a => a.Udb)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Asignatura?> GetByIdAsync(int id)
        {
            return await _context.Asignaturas
                .Include(a => a.Udb)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<bool> ExistsAsync(string nombre, Nivel nivel, int? udbId, int? excludeId = null)
        {
            return await _context.Asignaturas
                .AnyAsync(a => a.Nombre == nombre
                            && a.Nivel == nivel
                            && a.UdbId == udbId
                            && (excludeId == null || a.Id != excludeId));
        }
        public async Task AddAsync(Asignatura asignatura)
        {
            await _context.Asignaturas.AddAsync(asignatura);
        }
        public void Update(Asignatura asignatura)
        {
            _context.Asignaturas.Update(asignatura);
        }
        public void Delete(Asignatura asignatura)
        {
            _context.Asignaturas.Remove(asignatura);
        }

        public async Task CargarModulosAsync(Asignatura asignatura)
        {
            await _context.Entry(asignatura).Collection(a => a.AsignaturaModulos).LoadAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<bool> HasDetallesDesignacionAsync(int asignaturaId)
        {
            return await _context.Actividades
                .AnyAsync(a => a.AsignaturaId == asignaturaId);
        }
    }
}