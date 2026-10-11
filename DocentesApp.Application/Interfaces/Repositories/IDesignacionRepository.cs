

using DocentesApp.Domain.Entities;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IDesignacionRepository
    {
         // Trae todas las designaciones con sus prop de navegacion incluidas
        // (Docente, Detalles) para armar el listado
        Task<IEnumerable<Designacion>> GetAllAsync();
        // Trae designaciones filtradas por docente — para ver el historial de un docente
        Task<IEnumerable<Designacion>> GetByDocenteAsync(int docenteId);
        // Trae solo las designaciones vigentes (FechaFin IS NULL o FechaFin > hoy)
        Task<IEnumerable<Designacion>> GetVigentesAsync();
        // DIANA VER una para traer todas las designaciones vigentes de un docente, para ver si tiene alguna activa
        // Trae una designación con todos sus detalles incluidos
        Task<Designacion?> GetByIdAsync(int id);
        // [OBSOLETO v4.0] Regla 41: ya no existe restricción de unicidad docente-cargo.
        // Un docente puede tener múltiples designaciones activas con la misma categoría de cargo.
        // DIANA VER si se necesita alguna validación diferente a futuro
        // Task<bool> ExisteDesignacionActivaAsync(int docenteId, int cargoId, int? excludeId = null);
        // Suplencias (v5): cargos en licencia de una UDB sin suplente vigente, para el selector "Reemplaza a"
        Task<IEnumerable<CargoDesignacion>> GetCargosEnLicenciaAsync(int udbId);
        // Cargo por id con su designación, para validar el cargo que se reemplaza
        Task<CargoDesignacion?> GetCargoByIdAsync(int cargoId);
        // Regla (f): un cargo en licencia tiene a lo sumo un suplente vigente
        Task<bool> ExisteSuplenteActivoAsync(int cargoReemplazadoId, int? excludeCargoId = null);
        Task AddAsync(Designacion designacion);
        void Update(Designacion designacion);
        Task SaveChangesAsync();
    }
}
