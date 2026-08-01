using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;

namespace DocentesApp.Application.Interfaces.Repositories
{
    // [Diana desde v4.0 OBSOLETO]
    [Obsolete("ICargoRepository obsoleto desde v4.0.")]
    public interface ICargoRepository
    {
        Task<IEnumerable<Cargo>> GetAllAsync();
        Task<Cargo?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(DenominacionCargo denominacion, TipoCargo tipoCargo, Condicion condicion);
        // valida que la combinación de DenominacionCargo + TipoCargo + Condicion no exista para poder reutilizar
        Task AddAsync(Cargo cargo);
        void Delete(Cargo cargo);
        Task SaveChangesAsync();
        Task<bool> HasDesignacionesAsync(int cargoId);
    }
}