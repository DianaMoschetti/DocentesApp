using DocentesApp.Shared.DTOs.Cargos;

namespace DocentesApp.Application.Interfaces.Services
{
    // [Diana desde v4.0 OBSOLETO]
    [Obsolete("ICargoService obsoleto desde v4.0.")]
    public interface ICargoService
    {
        Task<IEnumerable<ListCargoDto>> GetAllAsync();
        Task<CargoDto> GetByIdAsync(int id);
        Task<CargoDto> CreateAsync(CreateCargoDto dto);
        Task DeleteAsync(int id);
    }
}