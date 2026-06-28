using DocentesApp.Shared.DTOs.Docentes;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IDocenteService
    {
        Task<IEnumerable<ListDocenteDto>> GetAllAsync();
        Task<DocenteDto> GetByIdAsync(int id);
        Task<DocenteDto> CreateAsync(CreateDocenteDto dto);
        Task UpdateAsync(int id, UpdateDocenteDto dto);       
        Task UpdateContactoAsync(int id, UpdateContactoDocenteDto dto);
        Task UpdateAcademicoAsync(int id, UpdateAcademicoDocenteDto dto);
        Task UpdateObservacionesAsync(int id, UpdateObservacionesDocenteDto dto);
        Task DeleteAsync(int id);
    }
}
