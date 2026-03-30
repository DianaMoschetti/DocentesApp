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
        public async Task<ActionResult<IEnumerable<ListDocenteDto>>> GetDocentes()
        {
            var result = await _docenteService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DocenteDto>> GetDocente(int id)
        {
            var result = await _docenteService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DocenteDto>> PostDocente(CreateDocenteDto dto)
        {
            var result = await _docenteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetDocente), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocente(int id, UpdateDocenteDto dto)
        {
            await _docenteService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocente(int id)
        {
            await _docenteService.DeleteAsync(id);
            return NoContent();
        }
    }
}