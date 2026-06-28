using DocentesApp.Shared.DTOs.Asignaturas;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IAsignaturaService
    {
        Task<IEnumerable<ListAsignaturaDto>> GetAllAsync();
        Task<AsignaturaDto> GetByIdAsync(int id);
        Task<AsignaturaDto> CreateAsync(CreateAsignaturaDto dto);
        Task UpdateAsync(int id, UpdateAsignaturaDto dto);
        Task DeleteAsync(int id);
    }
}
