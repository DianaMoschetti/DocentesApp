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