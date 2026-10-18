using DocentesApp.Shared.DTOs.Auditoria;

namespace DocentesApp.API.Services
{
    // Lectura del historial de cambios (solo Administrador)
    public interface IAuditoriaService
    {
        Task<PaginaHistorialCambioDto> BuscarAsync(FiltroAuditoriaDto filtro);
        Task<PaginaHistorialCambioDto> PorDocenteAsync(int docenteId, int pagina, int tamanioPagina);
        Task<PaginaHistorialCambioDto> PorDesignacionAsync(int designacionId, int pagina, int tamanioPagina);
        IEnumerable<EntidadAuditoriaDto> Entidades();
    }
}
