using DocentesApp.Shared.DTOs.Dedicaciones;

namespace DocentesApp.Application.Interfaces.Services
{
    // [Diana desde v4.0 OBSOLETO]
    [Obsolete("IDedicacionService obsoleto desde v4.0.")]
    public interface IDedicacionService
    {
        Task<IEnumerable<ListDedicacionDto>> GetAllAsync();
        Task<DedicacionDto> GetByIdAsync(int id);
        Task<DedicacionDto> CreateAsync(CreateDedicacionDto dto);
        Task DeleteAsync(int id);
    }
}