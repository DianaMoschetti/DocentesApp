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

        [HttpGet("planta/udb/{udbId}")]
        [ProducesResponseType(typeof(IEnumerable<PlantaDocenteReporteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlantaDocenteReporteDto>>> GetPlantaByUdb(int udbId)
        {
            var result = await _reporteService.GetPlantaByUdbAsync(udbId);
            return Ok(result);
        }

        [HttpGet("planta/docente/{docenteId}")]
        [ProducesResponseType(typeof(IEnumerable<PlantaDocenteReporteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PlantaDocenteReporteDto>>> GetPlantaByDocente(int docenteId)
        {
            var result = await _reporteService.GetPlantaByDocenteAsync(docenteId);
            return Ok(result);
        }
    }
}