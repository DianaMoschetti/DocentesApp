using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly DocentesDbContext _context;

        public CursoRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _context.Cursos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Curso?> GetByIdAsync(int id)
        {
            return await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(Turno turno, Nivel año, Especialidad carrera, int nroComision, int? excludeId = null)
        {
            return await _context.Cursos
                .AnyAsync(c => c.Turno == turno
                            && c.Año == año
                            && c.Carrera == carrera
                            && c.NroComision == nroComision
                            && (excludeId == null || c.Id != excludeId));
        }

        public async Task AddAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
        }

        public void Update(Curso curso)
        {
            _context.Cursos.Update(curso);
        }

        public void Delete(Curso curso)
        {
            _context.Cursos.Remove(curso);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasDependenciasAsync(int cursoId)
        {
            var tieneModulos = await _context.AsignaturaModulos
                .AnyAsync(am => am.CursoId == cursoId);

            if (tieneModulos) return true;

            return await _context.DetalleDesignaciones
                .AnyAsync(d => d.CursoId == cursoId);
        }
    }
}
