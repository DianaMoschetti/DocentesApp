using DocentesApp.Shared.DTOs.Dedicaciones;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IDedicacionService
    {
        Task<IEnumerable<ListDedicacionDto>> GetAllAsync();
        Task<DedicacionDto> GetByIdAsync(int id);
        Task<DedicacionDto> CreateAsync(CreateDedicacionDto dto);
        Task DeleteAsync(int id);
    }
}