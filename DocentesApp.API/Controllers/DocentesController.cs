using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly IDocenteService _docenteService;

        public DocentesController(IDocenteService docenteService)
        {
            _docenteService = docenteService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListDocenteDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListDocenteDto>>> GetDocentes()
        {
            var result = await _docenteService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DocenteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocenteDto>> GetDocente(int id)
        {
            var result = await _docenteService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DocenteDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DocenteDto>> PostDocente([FromBody] CreateDocenteDto dto)
        {
            var result = await _docenteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDocente), new { id = result.Id }, result);
        }

        [HttpPut("{id}")] // reemplaza todo, si va en null se pisa con null.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDocente(int id, [FromBody] UpdateDocenteDto dto)
        {
            await _docenteService.UpdateAsync(id, dto);
            return NoContent();
        }

        // Updates parciales
        [HttpPatch("{id}/contacto")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateContacto(int id, [FromBody] UpdateContactoDocenteDto dto)
        {
            await _docenteService.UpdateContactoAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/academico")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAcademico(int id, [FromBody] UpdateAcademicoDocenteDto dto)
        {
            await _docenteService.UpdateAcademicoAsync(id, dto);
            return NoContent();
        }

        [HttpPatch("{id}/observaciones")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateObservaciones(int id, [FromBody] UpdateObservacionesDocenteDto dto)
        {
            await _docenteService.UpdateObservacionesAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteDocente(int id)
        {
            await _docenteService.DeleteAsync(id);
            return NoContent();
        }
    }
}