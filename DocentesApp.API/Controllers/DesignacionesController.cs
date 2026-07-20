using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Designaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignacionesController : ControllerBase
    {
        private readonly IDesignacionService _designacionService;

        public DesignacionesController(IDesignacionService designacionService)
        {
            _designacionService = designacionService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListDesignacionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListDesignacionDto>>> GetDesignaciones()
        {
            var result = await _designacionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("vigentes")]
        [ProducesResponseType(typeof(IEnumerable<ListDesignacionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListDesignacionDto>>> GetVigentes()
        {
            var result = await _designacionService.GetVigentesAsync();
            return Ok(result);
        }

        [HttpGet("docente/{docenteId}")]
        [ProducesResponseType(typeof(IEnumerable<ListDesignacionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ListDesignacionDto>>> GetByDocente(int docenteId)
        {
            var result = await _designacionService.GetByDocenteAsync(docenteId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DesignacionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DesignacionDto>> GetDesignacion(int id)
        {
            var result = await _designacionService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DesignacionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DesignacionDto>> PostDesignacion([FromBody] CreateDesignacionDto dto)
        {
            var result = await _designacionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDesignacion), new { id = result.Id }, result);
        }

        #endregion

        #region Update

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDesignacion(int id, [FromBody] UpdateDesignacionDto dto)
        {
            await _designacionService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/cerrar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CerrarDesignacion(int id)
        {
            await _designacionService.CerrarAsync(id);
            return NoContent();
        }

        #endregion
    }
}