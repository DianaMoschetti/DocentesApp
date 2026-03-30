using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignacionesController : ControllerBase
    {
        private readonly DocentesDbContext _context;

        public DesignacionesController(DocentesDbContext context)
        {
            _context = context;
        }

        // diana revisar: Cuando devuelvas una designación como DTO, cargá las relaciones con Include, si no esos textos pueden venir vacíos.
        /*
         * var designacion = await _context.Designaciones
    .Include(d => d.Docente)
    .Include(d => d.Cargo)
    .Include(d => d.Dedicacion)
    .Include(d => d.Asignatura)
    .Include(d => d.Curso)
    .FirstOrDefaultAsync(d => d.Id == id);

        y recien despues:
        var dto = designacion.Adapt<DesignacionDto>();
         */
        // GET: api/Designaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Designacion>>> GetDesignaciones()
        {
            return await _context.Designaciones.ToListAsync();
        }

        // GET: api/Designaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Designacion>> GetDesignacion(int id)
        {
            var designacion = await _context.Designaciones.FindAsync(id);

            if (designacion == null)
            {
                return NotFound();
            }

            return designacion;
        }

        // PUT: api/Designaciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDesignacion(int id, Designacion designacion)
        {
            if (id != designacion.Id)
            {
                return BadRequest();
            }

            _context.Entry(designacion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DesignacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Designaciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Designacion>> PostDesignacion(Designacion designacion)
        {
            _context.Designaciones.Add(designacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDesignacion", new { id = designacion.Id }, designacion);
        }

        // DELETE: api/Designaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesignacion(int id)
        {
            var designacion = await _context.Designaciones.FindAsync(id);
            if (designacion == null)
            {
                return NotFound();
            }

            _context.Designaciones.Remove(designacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DesignacionExists(int id)
        {
            return _context.Designaciones.Any(e => e.Id == id);
        }
    }
}
