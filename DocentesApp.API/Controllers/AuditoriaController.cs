using DocentesApp.API.Services;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auditoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    // Historial de cambios: solo Administrador. Siempre paginado y del más nuevo al más viejo.
    // pagina >= 1 (si no, 400); tamanioPagina >= 1 (si no, 400) y como máximo 200 (más se recorta a 200).
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.EsAdministrador)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _auditoriaService;

        public AuditoriaController(IAuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        // Filtros: usuarioId (o "sistema" para las filas sin usuario), desde/hasta (días de Argentina, hasta inclusive),
        // entidad (clave de GET api/auditoria/entidades) y accion
        [HttpGet]
        [ProducesResponseType(typeof(PaginaHistorialCambioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginaHistorialCambioDto>> GetHistorial([FromQuery] FiltroAuditoriaDto filtro)
        {
            return Ok(await _auditoriaService.BuscarAsync(filtro));
        }

        // ruta "docente/{docenteId}": NSwag nombra los métodos por el último segmento y
        // "DocenteAsync" ya era el de api/designaciones/docente/{id} (lo renombraba). Ahora "historial-docente" -> HistorialDocenteAsync
        // Datos del docente, sus membresías a UDB y sus designaciones con sus cargos y actividades
        [HttpGet("historial-docente/{docenteId:int}")]
        [ProducesResponseType(typeof(PaginaHistorialCambioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginaHistorialCambioDto>> GetHistorialDocente(
            int docenteId, [FromQuery] int pagina = 1, [FromQuery] int tamanioPagina = FiltroAuditoriaDto.TamanioPaginaPorDefecto)
        {
            return Ok(await _auditoriaService.PorDocenteAsync(docenteId, pagina, tamanioPagina));
        }

        // ruta "designacion/{designacionId}": mismo motivo que historial-docente
        // La designación, sus cargos y sus actividades
        [HttpGet("historial-designacion/{designacionId:int}")]
        [ProducesResponseType(typeof(PaginaHistorialCambioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginaHistorialCambioDto>> GetHistorialDesignacion(
            int designacionId, [FromQuery] int pagina = 1, [FromQuery] int tamanioPagina = FiltroAuditoriaDto.TamanioPaginaPorDefecto)
        {
            return Ok(await _auditoriaService.PorDesignacionAsync(designacionId, pagina, tamanioPagina));
        }

        // Opciones del filtro por entidad (clave + etiqueta)
        [HttpGet("entidades")]
        [ProducesResponseType(typeof(IEnumerable<EntidadAuditoriaDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EntidadAuditoriaDto>> GetEntidades()
        {
            return Ok(_auditoriaService.Entidades());
        }
    }
}
