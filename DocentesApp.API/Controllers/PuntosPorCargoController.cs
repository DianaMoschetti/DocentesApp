using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuntosPorCargoController : ControllerBase
    {
        private readonly IPuntosPorCargoService _puntosPorCargoService;

        public PuntosPorCargoController(IPuntosPorCargoService puntosPorCargoService)
        {
            _puntosPorCargoService = puntosPorCargoService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PuntosPorCargoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PuntosPorCargoDto>>> GetPuntosPorCargo()
        {
            var result = await _puntosPorCargoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PuntosPorCargoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PuntosPorCargoDto>> GetPuntosPorCargo(int id)
        {
            var result = await _puntosPorCargoService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PuntosPorCargoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PuntosPorCargoDto>> PostPuntosPorCargo([FromBody] CreatePuntosPorCargoDto dto)
        {
            var result = await _puntosPorCargoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPuntosPorCargo), new { id = result.Id }, result);
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeletePuntosPorCargo(int id)
        {
            await _puntosPorCargoService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
