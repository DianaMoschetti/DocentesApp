using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Dedicaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DedicacionesController : ControllerBase
    {
        private readonly IDedicacionService _dedicacionService;

        public DedicacionesController(IDedicacionService dedicacionService)
        {
            _dedicacionService = dedicacionService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListDedicacionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListDedicacionDto>>> GetDedicaciones()
        {
            var result = await _dedicacionService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DedicacionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DedicacionDto>> GetDedicacion(int id)
        {
            var result = await _dedicacionService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(DedicacionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DedicacionDto>> PostDedicacion([FromBody] CreateDedicacionDto dto)
        {
            var result = await _dedicacionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDedicacion), new { id = result.Id }, result);
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
        public async Task<IActionResult> DeleteDedicacion(int id)
        {
            await _dedicacionService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}