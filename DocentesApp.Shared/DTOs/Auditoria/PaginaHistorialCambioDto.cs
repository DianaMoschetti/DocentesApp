namespace DocentesApp.Shared.DTOs.Auditoria
{
    // Página de historial, como tipo concreto: el genérico PaginaDto<T> sale en swagger como "HistorialCambioDtoPaginaDto",
    // un tipo que no existe en Shared, y el cliente NSwag (generateDtoTypes = false) no compila. Con este nombre real, sí.
    public class PaginaHistorialCambioDto : PaginaDto<HistorialCambioDto>
    {
    }
}
