using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Shared.DTOs.Cursos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursosController : ControllerBase
    {
        private readonly ICursoService _cursoService;

        public CursosController(ICursoService cursoService)
        {
            _cursoService = cursoService;
        }

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListCursoDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListCursoDto>>> GetCursos()
        {
            var result = await _cursoService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CursoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CursoDto>> GetCurso(int id)
        {
            var result = await _cursoService.GetByIdAsync(id);
            return Ok(result);
        }

        #endregion

        #region Create

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CursoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CursoDto>> PostCurso([FromBody] CreateCursoDto dto)
        {
            var result = await _cursoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCurso), new { id = result.Id }, result);
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
        public async Task<IActionResult> PutCurso(int id, [FromBody] UpdateCursoDto dto)
        {
            await _cursoService.UpdateAsync(id, dto);
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
        public async Task<IActionResult> DeleteCurso(int id)
        {
            await _cursoService.DeleteAsync(id);
            return NoContent();
        }

        #endregion
    }
}
