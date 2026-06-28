using DocentesApp.Shared.DTOs.Udbs;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IUdbService
    {
        Task<IEnumerable<ListUdbDto>> GetAllAsync();
        Task<UdbDto> GetByIdAsync(int id);
        Task<UdbDto> CreateAsync(CreateUdbDto dto);
        Task UpdateAsync(int id, UpdateUdbDto dto);
        Task DeleteAsync(int id);
    }
}
