using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Data.Context;
using DocentesApp.Model;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly DocentesDbContext _context;

        public DocentesController(DocentesDbContext context)
        {
            _context = context;
        }

        // GET: api/Docentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListDocenteDto>>> GetDocentes()
        {
            var docentes = await _context.Docentes.ToListAsync();
            var docentesDto = docentes.Adapt<List<ListDocenteDto>>();

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

            var docenteDto = docente.Adapt<DocenteDto>();

            return Ok(docenteDto);
        }

        // GET: api/Docentes/5/detalle
        [HttpGet("{id}/detalle")]
        public async Task<ActionResult<DetalleDocenteDto>> GetDocenteDetalle(int id)
        {
            var docente = await _context.Docentes
                .Include(d => d.Designaciones)
                    .ThenInclude(des => des.Docente)
                .Include(d => d.Designaciones)
                    .ThenInclude(des => des.Cargo)
                .Include(d => d.Designaciones)
                    .ThenInclude(des => des.Dedicacion)
                .Include(d => d.Designaciones)
                    .ThenInclude(des => des.Asignatura)
                .Include(d => d.Designaciones)
                    .ThenInclude(des => des.Curso)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            var docenteDetalleDto = docente.Adapt<DetalleDocenteDto>();

            return Ok(docenteDetalleDto);
        }

        // PUT: api/Docentes/5
        // Los campos que no mande van a quedar en null
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocente(int id, UpdateDocenteDto docenteDto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            docenteDto.Adapt(docente);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Docentes/5/academico
        [HttpPatch("{id}/academico")]
        public async Task<IActionResult> PatchDocenteAcademico(int id, UpdateAcademicoDocenteDto docenteDto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            docenteDto.Adapt(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Docentes/5/contacto
        [HttpPatch("{id}/contacto")]
        public async Task<IActionResult> PatchDocenteContacto(int id, UpdateContactoDocenteDto docenteDto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            docenteDto.Adapt(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/Docentes/5/observaciones
        [HttpPatch("{id}/observaciones")]
        public async Task<IActionResult> PatchDocenteObservaciones(int id, UpdateObservacionesDocenteDto docenteDto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            docenteDto.Adapt(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Docentes
        [HttpPost]
        public async Task<ActionResult<DocenteDto>> PostDocente(CreateDocenteDto docenteDto)
        {
            var yaExisteLegajo = await _context.Docentes.AnyAsync(d => d.Legajo == docenteDto.Legajo);
            if (yaExisteLegajo)
                throw new BadRequestException("Ya existe un docente con ese legajo.");

            var docente = docenteDto.Adapt<Docente>();

            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            var docenteCreado = docente.Adapt<DocenteDto>();

            return CreatedAtAction(nameof(GetDocente), new { id = docente.Id }, docenteCreado);
        }

        // DELETE: api/Docentes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocente(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con Id {id}.");

            var tieneDesignaciones = await _context.Designaciones.AnyAsync(d => d.DocenteId == id);
            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar el docente porque tiene designaciones asociadas.");

            _context.Docentes.Remove(docente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
