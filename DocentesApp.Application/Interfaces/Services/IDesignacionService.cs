
using DocentesApp.Shared.DTOs.Designaciones;

namespace DocentesApp.Application.Interfaces.Services
{
    public interface IDesignacionService
    {
        Task<IEnumerable<ListDesignacionDto>> GetAllAsync();
        Task<IEnumerable<ListDesignacionDto>> GetVigentesAsync();
        // [Diana desde v4.0 OBSOLETO] devuelve cabecera + cargos (DetalleDocente)
        //Task<IEnumerable<ListDesignacionDto>> GetByDocenteAsync(int docenteId);
        Task<IEnumerable<DesignacionDto>> GetByDocenteAsync(int docenteId);
        Task<DesignacionDto> GetByIdAsync(int id);
        Task<DesignacionDto> CreateAsync(CreateDesignacionDto dto);
        Task UpdateAsync(int id, UpdateDesignacionDto dto);
        Task CerrarAsync(int id);
        // Opciones del selector "Reemplaza a" (cargos en licencia de la UDB sin suplente vigente)
        Task<IEnumerable<CargoEnLicenciaDto>> GetCargosEnLicenciaAsync(int udbId);
    }
}
