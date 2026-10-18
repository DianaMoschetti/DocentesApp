using DocentesApp.Shared.DTOs.Auditoria;

namespace DocentesApp.API.Services
{
    // Lectura del historial de cambios (solo Administrador)
    public interface IAuditoriaService
    {
        // [Diana desde v4.0 OBSOLETO] devolvían el genérico PaginaDto<HistorialCambioDto> (ver PaginaHistorialCambioDto)
        //Task<PaginaDto<HistorialCambioDto>> BuscarAsync(FiltroAuditoriaDto filtro);
        //Task<PaginaDto<HistorialCambioDto>> PorDocenteAsync(int docenteId, int pagina, int tamanioPagina);
        //Task<PaginaDto<HistorialCambioDto>> PorDesignacionAsync(int designacionId, int pagina, int tamanioPagina);
        Task<PaginaHistorialCambioDto> BuscarAsync(FiltroAuditoriaDto filtro);
        Task<PaginaHistorialCambioDto> PorDocenteAsync(int docenteId, int pagina, int tamanioPagina);
        Task<PaginaHistorialCambioDto> PorDesignacionAsync(int designacionId, int pagina, int tamanioPagina);
        IEnumerable<EntidadAuditoriaDto> Entidades();
    }
}
