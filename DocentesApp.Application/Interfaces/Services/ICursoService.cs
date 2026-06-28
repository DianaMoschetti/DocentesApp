using DocentesApp.Shared.DTOs.Cursos;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface ICursoService
    {
        Task<IEnumerable<ListCursoDto>> GetAllAsync();
        Task<CursoDto> GetByIdAsync(int id);
        Task<CursoDto> CreateAsync(CreateCursoDto dto);
        Task UpdateAsync(int id, UpdateCursoDto dto);
        Task DeleteAsync(int id);
    }
}
