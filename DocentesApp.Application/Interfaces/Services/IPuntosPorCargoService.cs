using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.PuntosPorCargo;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IPuntosPorCargoService
    {
        Task<IEnumerable<PuntosPorCargoDto>> GetAllAsync();
        Task<PuntosPorCargoDto> GetByIdAsync(int id);
        Task<PuntosPorCargoDto> CreateAsync(CreatePuntosPorCargoDto dto);
        Task DeleteAsync(int id);
        Task<decimal> GetPuntosAsync(DenominacionCargo denominacion, TipoCargo tipoCargo);
    }
}
