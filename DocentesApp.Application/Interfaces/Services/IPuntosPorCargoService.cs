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
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser CargoDocente
        //Task<decimal> GetPuntosAsync(DenominacionCargo denominacion, TipoCargo tipoCargo);
        Task<decimal> GetPuntosAsync(CargoDocente cargo);
    }
}
