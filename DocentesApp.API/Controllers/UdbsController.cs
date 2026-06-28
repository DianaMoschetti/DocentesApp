using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Udbs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UdbsController : ControllerBase
    {
        private readonly IUdbService _udbService;

        public UdbsController(IUdbService udbService)
        {
            _udbService = udbService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListUdbDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListUdbDto>>> GetUdbs()
        {
            var result = await _udbService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UdbDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UdbDto>> GetUdb(int id)
        {
            var result = await _udbService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UdbDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UdbDto>> PostUdb([FromBody] CreateUdbDto dto)
        {
            var result = await _udbService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUdb), new { id = result.Id }, result);
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
        public async Task<IActionResult> PutUdb(int id, [FromBody] UpdateUdbDto dto)
        {
            await _udbService.UpdateAsync(id, dto);
            return NoContent();
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
        public async Task<IActionResult> DeleteUdb(int id)
        {
            await _udbService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
