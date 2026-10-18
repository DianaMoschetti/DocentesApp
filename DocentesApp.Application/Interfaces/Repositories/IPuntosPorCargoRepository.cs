using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IPuntosPorCargoRepository
    {
        Task<IEnumerable<PuntosPorCargo>> GetAllAsync();
        Task<PuntosPorCargo?> GetByIdAsync(int id);
        Task<PuntosPorCargo?> GetByCargoAsync(CargoDocente cargo);
        // true si algún CargoDesignacion usa ese cargo (no se puede borrar la fila del catálogo)
        Task<bool> EstaEnUsoAsync(CargoDocente cargo);
        Task AddAsync(PuntosPorCargo puntosPorCargo);
        void Delete(PuntosPorCargo puntosPorCargo);
        Task SaveChangesAsync();
    }
}
