using DocentesApp.Shared.DTOs.Cargos;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface ICargoService
    {
        Task<IEnumerable<ListCargoDto>> GetAllAsync();
        Task<CargoDto> GetByIdAsync(int id);
        Task<CargoDto> CreateAsync(CreateCargoDto dto);
        Task DeleteAsync(int id);
    }
}