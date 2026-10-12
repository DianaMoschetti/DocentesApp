using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Reportes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // cualquier usuario autenticado puede ver los reportes: admins y users
    public class ReportesController : ControllerBase
    {
        private readonly IPlantaReporteService _reporteService;

        public ReportesController(IPlantaReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // [Diana desde v4.0 OBSOLETO] reemplazado por planta/udb/{udbId}/por-asignatura y /por-docente (v5)
        //[HttpGet("planta/udb/{udbId}")]
        //[ProducesResponseType(typeof(IEnumerable<PlantaDocenteReporteDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<IEnumerable<PlantaDocenteReporteDto>>> GetPlantaByUdb(int udbId)
        //{
        //    var result = await _reporteService.GetPlantaByUdbAsync(udbId);
        //    return Ok(result);
        //}

        [HttpGet("planta/udb/{udbId}/por-asignatura")]
        [ProducesResponseType(typeof(PlantaUdbReporteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlantaUdbReporteDto>> GetPlantaUdbPorAsignatura(int udbId)
        {
            var result = await _reporteService.GetPlantaUdbPorAsignaturaAsync(udbId);
            return Ok(result);
        }

        [HttpGet("planta/udb/{udbId}/por-docente")]
        [ProducesResponseType(typeof(PlantaUdbReporteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlantaUdbReporteDto>> GetPlantaUdbPorDocente(int udbId)
        {
            var result = await _reporteService.GetPlantaUdbPorDocenteAsync(udbId);
            return Ok(result);
        }

        // [Diana desde v4.0 OBSOLETO] devolvía filas PlantaDocenteReporteDto; ahora un reporte por UDB del docente
        //[HttpGet("planta/docente/{docenteId}")]
        //[ProducesResponseType(typeof(IEnumerable<PlantaDocenteReporteDto>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult<IEnumerable<PlantaDocenteReporteDto>>> GetPlantaByDocente(int docenteId)
        //{
        //    var result = await _reporteService.GetPlantaByDocenteAsync(docenteId);
        //    return Ok(result);
        //}

        [HttpGet("planta/docente/{docenteId}")]
        [ProducesResponseType(typeof(IEnumerable<PlantaUdbReporteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlantaUdbReporteDto>>> GetPlantaByDocente(int docenteId)
        {
            var result = await _reporteService.GetPlantaByDocenteAsync(docenteId);
            return Ok(result);
        }
    }
}