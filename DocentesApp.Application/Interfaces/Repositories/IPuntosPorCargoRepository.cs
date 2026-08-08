using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    public interface IPuntosPorCargoRepository
    {
        Task<IEnumerable<PuntosPorCargo>> GetAllAsync();
        Task<PuntosPorCargo?> GetByIdAsync(int id);
        Task<PuntosPorCargo?> GetByCargoAsync(DenominacionCargo denominacion, TipoCargo tipoCargo);
        Task AddAsync(PuntosPorCargo puntosPorCargo);
        void Delete(PuntosPorCargo puntosPorCargo);
        Task SaveChangesAsync();
    }
}
