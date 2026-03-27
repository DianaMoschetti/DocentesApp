using AutoMapper;
using DocentesApp.Application.Common.Constants;
using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Data.Context;
using DocentesApp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly DocentesDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DocentesController(DocentesDbContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Docentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListDocenteDto>>> GetDocentes()
        {
            var docentes = await _context.Docentes.ToListAsync();
            var docentesDto = _mapper.Map<List<ListDocenteDto>>(docentes);

            return Ok(docentesDto);
        }

        // GET: api/Docentes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocenteDto>> GetDocente(int id)
        {
            var docente = await _context.Docentes
                .FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            var docenteDto = _mapper.Map<DocenteDto>(docente);

            return Ok(docenteDto);
        }

        // PUT: api/Docentes/5
        // Los campos que no mande van a quedar en null
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocente(int id, UpdateDocenteDto docenteDto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                return NotFound();

            _mapper.Map(docenteDto, docente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               _logger.LogError(ex,
                    "{Message} Controller: {Controller}, Action: {Action}, TraceId: {TraceId}",
                    LogMessages.ErrorLogUnhandled
                    );
                return StatusCode(500);
            }
            
            return NoContent();
        }

        // POST: api/Docentes
        [HttpPost]
        public async Task<ActionResult<DocenteDto>> PostDocente(CreateDocenteDto docenteDto)
        {
            var yaExisteLegajo = await _context.Docentes.AnyAsync(d => d.Legajo == docenteDto.Legajo);
            if (yaExisteLegajo)
                throw new BadRequestException("Ya existe un docente con ese legajo.");

            var docente = _mapper.Map<Docente>(docenteDto);

            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            var docenteCreado= _mapper.Map<DocenteDto>(docente);

            return CreatedAtAction(nameof(GetDocente), new { id = docente.Id }, docenteCreado);
        }

        // DELETE: api/Docentes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocente(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con Id {id}.");

            var tieneDesignaciones = await _context.Designaciones.AnyAsync(d => d.DocenteId == id); // ver si esta activa
            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar el docente porque tiene designaciones asociadas.");

            _context.Docentes.Remove(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> DocenteExists(int id)
        {
            return await _context.Docentes.AnyAsync(e => e.Id == id);
        }
    }
}