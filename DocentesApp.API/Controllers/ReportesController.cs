using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Enums;
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
        private readonly IDocentesPorCursoService _docentesPorCursoService;

        public ReportesController(IPlantaReporteService reporteService, IDocentesPorCursoService docentesPorCursoService)
        {
            _reporteService = reporteService;
            _docentesPorCursoService = docentesPorCursoService;
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

        // Vista por curso: cursos con sus asignaturas y docentes de cargos vigentes. Filtros opcionales (se combinan).
        [HttpGet("docentes-por-curso")]
        [ProducesResponseType(typeof(IEnumerable<CursoDocentesDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CursoDocentesDto>>> GetDocentesPorCurso(
            [FromQuery] Especialidad? carrera, [FromQuery] Nivel? anio, [FromQuery] int? cursoId)
        {
            var result = await _docentesPorCursoService.GetPorCursoAsync(carrera, anio, cursoId);
            return Ok(result);
        }

        [HttpGet("docentes-por-asignatura/{asignaturaId}")]
        [ProducesResponseType(typeof(AsignaturaDocentesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AsignaturaDocentesDto>> GetDocentesPorAsignatura(int asignaturaId)
        {
            var result = await _docentesPorCursoService.GetPorAsignaturaAsync(asignaturaId);
            return Ok(result);
        }
    }
}