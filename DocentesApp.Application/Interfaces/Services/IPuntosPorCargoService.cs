using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.PuntosPorCargo;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IPuntosPorCargoService
    {
        Task<IEnumerable<PuntosPorCargoDto>> GetAllAsync();
        Task<PuntosPorCargoDto> GetByIdAsync(int id);
        Task<PuntosPorCargoDto> CreateAsync(CreatePuntosPorCargoDto dto);
        Task UpdateAsync(int id, UpdatePuntosPorCargoDto dto); // solo PuntosBase; no recalcula designaciones existentes
        Task DeleteAsync(int id); // 409 si el cargo se usa en designaciones
        // [Diana desde v4.0 OBSOLETO] la clave pasa a ser CargoDocente
        //Task<decimal> GetPuntosAsync(DenominacionCargo denominacion, TipoCargo tipoCargo);
        Task<decimal> GetPuntosAsync(CargoDocente cargo);
    }
}
