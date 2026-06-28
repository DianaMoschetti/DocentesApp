using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Cargos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargosController : ControllerBase
    {
        private readonly ICargoService _cargoService;

        public CargosController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListCargoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListCargoDto>>> GetCargos()
        {
            var result = await _cargoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CargoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CargoDto>> GetCargo(int id)
        {
            var result = await _cargoService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CargoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CargoDto>> PostCargo([FromBody] CreateCargoDto dto)
        {
            var result = await _cargoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCargo), new { id = result.Id }, result);
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
        public async Task<IActionResult> DeleteCargo(int id)
        {
            await _cargoService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}