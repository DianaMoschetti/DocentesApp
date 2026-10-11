using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
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
                // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos -> Actividades (v5)
                //.Include(d => d.Detalles)
                .Include(d => d.Cargos)
                .OrderByDescending(d => d.FechaInicio)
                .ToListAsync();
        }
        public async Task<IEnumerable<Designacion>> GetByDocenteAsync(int docenteId)
        {
            return await _context.Designaciones
                .AsNoTracking()
                // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos -> Actividades (v5)
                //.Include(d => d.Detalles)
                .Include(d => d.Cargos)
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
                // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos -> Actividades (v5)
                //.Include(d => d.Detalles)
                .Include(d => d.Cargos)
                // [Diana desde v4.0 OBSOLETO] vigente = Activa y sin FechaFin o con FechaFin futura
                //.Where(d => d.FechaFin == null || d.FechaFin > hoy)
                .Where(d => d.EstadoDesignacion == Estado.Activa && (d.FechaFin == null || d.FechaFin > hoy))
                .OrderBy(d => d.Docente.Apellido)
                .ToListAsync();
        }
        public async Task<Designacion?> GetByIdAsync(int id)
        {
            return await _context.Designaciones
                .Include(d => d.Docente)
                // [Diana desde v4.0 OBSOLETO] Detalles reemplazado por Cargos -> Actividades (v5)
                //.Include(d => d.Detalles)
                //    .ThenInclude(det => det.Asignatura)
                //.Include(d => d.Detalles)
                //    .ThenInclude(det => det.Curso)
                .Include(d => d.Cargos)
                    .ThenInclude(c => c.Udb)
                .Include(d => d.Cargos)
                    .ThenInclude(c => c.Actividades)
                        .ThenInclude(a => a.Asignatura)
                .Include(d => d.Cargos)
                    .ThenInclude(c => c.Actividades)
                        .ThenInclude(a => a.Curso)
                .Include(d => d.Cargos)
                    .ThenInclude(c => c.ReemplazaACargo)
                        .ThenInclude(r => r!.Designacion)
                            .ThenInclude(rd => rd.Docente)
                .AsSplitQuery() // varios niveles de colecciones: evita el producto cartesiano
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        // [Diana refactor v4] Ya no existe restricción de unicidad docente-cargo (Regla 41)
        // public async Task<bool> ExisteDesignacionActivaAsync(int docenteId, int cargoId, int? excludeId = null)
        // {
        //     var hoy = DateTime.Now;
        //     return await _context.Designaciones
        //         .AnyAsync(d =>
        //             d.DocenteId == docenteId &&
        //             d.CargoId == cargoId &&
        //             (d.FechaFin == null || d.FechaFin > hoy) &&
        //             (excludeId == null || d.Id != excludeId));
        // }
        // Cargos en licencia de una UDB, en designaciones vigentes y sin suplente vigente:
        // son los candidatos para el selector "Reemplaza a" de un cargo suplente.
        public async Task<IEnumerable<CargoDesignacion>> GetCargosEnLicenciaAsync(int udbId)
        {
            var hoy = DateTime.Now;
            return await _context.CargosDesignacion
                .AsNoTracking()
                .Include(c => c.Designacion)
                    .ThenInclude(d => d.Docente)
                .Where(c => c.UdbId == udbId
                    && c.EnLicencia
                    && c.Designacion.EstadoDesignacion == Estado.Activa
                    && (c.Designacion.FechaFin == null || c.Designacion.FechaFin > hoy)
                    && !c.Reemplazos.Any(r => r.Designacion.EstadoDesignacion == Estado.Activa
                        && (r.Designacion.FechaFin == null || r.Designacion.FechaFin > hoy)))
                .OrderBy(c => c.Designacion.Docente.Apellido)
                .ThenBy(c => c.Designacion.Docente.Nombre)
                .ToListAsync();
        }
        // Cargo por id con su designación (para validar el cargo que se reemplaza)
        public async Task<CargoDesignacion?> GetCargoByIdAsync(int cargoId)
        {
            return await _context.CargosDesignacion
                .AsNoTracking()
                .Include(c => c.Designacion)
                .FirstOrDefaultAsync(c => c.Id == cargoId);
        }
        // true si el cargo ya tiene un suplente en una designación vigente (excluyendo un cargo, para la edición)
        public async Task<bool> ExisteSuplenteActivoAsync(int cargoReemplazadoId, int? excludeCargoId = null)
        {
            var hoy = DateTime.Now;
            return await _context.CargosDesignacion
                .AnyAsync(c => c.ReemplazaACargoId == cargoReemplazadoId
                    && (excludeCargoId == null || c.Id != excludeCargoId)
                    && c.Designacion.EstadoDesignacion == Estado.Activa
                    && (c.Designacion.FechaFin == null || c.Designacion.FechaFin > hoy));
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