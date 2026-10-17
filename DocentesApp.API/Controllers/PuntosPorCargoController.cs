using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.PuntosPorCargo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocentesApp.Shared.Auth;

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
        // [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por politicas
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Politicas.PuedeEditar)]
        [ProducesResponseType(typeof(PuntosPorCargoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // ya hay puntos para ese cargo
        public async Task<ActionResult<PuntosPorCargoDto>> PostPuntosPorCargo([FromBody] CreatePuntosPorCargoDto dto)
        {
            var result = await _puntosPorCargoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPuntosPorCargo), new { id = result.Id }, result);
        }

        #endregion

        #region Update

        // Solo cambia PuntosBase; no recalcula los puntos guardados en designaciones existentes
        [HttpPut("{id}")]
        // [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por politicas
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Politicas.PuedeEditar)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutPuntosPorCargo(int id, [FromBody] UpdatePuntosPorCargoDto dto)
        {
            await _puntosPorCargoService.UpdateAsync(id, dto);
            return NoContent();
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        // [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por politicas
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Politicas.PuedeEditar)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Diana desde v4.0 OBSOLETO] ahora documenta el cuerpo ProblemDetails del 409
        //[ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // el cargo se usa en designaciones
        public async Task<IActionResult> DeletePuntosPorCargo(int id)
        {
            await _puntosPorCargoService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
