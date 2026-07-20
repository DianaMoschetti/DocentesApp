using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class DesignacionRepository : IDesignacionRepository
    {
        private readonly DocentesDbContext _context;

        public DesignacionRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Designacion>> GetAllAsync()
        {
            return await _context.Designaciones
                .AsNoTracking()
                .Include(d => d.Docente)
                .Include(d => d.Cargo)
                .Include(d => d.Dedicacion)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Designacion>> GetByDocenteAsync(int docenteId)
        {
            return await _context.Designaciones
                .AsNoTracking()
                .Include(d => d.Cargo)
                .Include(d => d.Dedicacion)
                .Where(d => d.DocenteId == docenteId)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Designacion>> GetVigentesAsync()
        {
            var hoy = DateTime.Now;
            return await _context.Designaciones
                .AsNoTracking()
                .Include(d => d.Docente)
                .Include(d => d.Cargo)
                .Include(d => d.Dedicacion)
                .Where(d => d.FechaFin == null || d.FechaFin > hoy)
                .OrderBy(d => d.Docente.Apellido)
                .ToListAsync();
        }

        public async Task<Designacion?> GetByIdAsync(int id)
        {
            return await _context.Designaciones
                .Include(d => d.Docente)
                .Include(d => d.Cargo)
                .Include(d => d.Dedicacion)
                .Include(d => d.Detalles)
                    .ThenInclude(det => det.Asignatura)
                .Include(d => d.Detalles)
                    .ThenInclude(det => det.Curso)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<bool> ExisteDesignacionActivaAsync(int docenteId, int cargoId, int? excludeId = null)
        {
            var hoy = DateTime.Now;
            return await _context.Designaciones
                .AnyAsync(d =>
                    d.DocenteId == docenteId &&
                    d.CargoId == cargoId &&
                    (d.FechaFin == null || d.FechaFin > hoy) &&
                    (excludeId == null || d.Id != excludeId));
        }

        public async Task AddAsync(Designacion designacion)
        {
            await _context.Designaciones.AddAsync(designacion);
        }

        public void Update(Designacion designacion)
        {
            _context.Designaciones.Update(designacion);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
