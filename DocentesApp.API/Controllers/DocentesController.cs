using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocentesApp.Data.Context;
using DocentesApp.Model;
using AutoMapper;
using DocentesApp.Application.DTOs.Docentes;

namespace DocentesApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly DocentesDbContext _context;
        private readonly IMapper mapper;

        public DocentesController(DocentesDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        // GET: api/Docentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Docente>>> GetDocentes()
        {
            return await _context.Docentes.ToListAsync();
        }

        // GET: api/Docentes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Docente>> GetDocente(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
            {
                return NotFound();
            }

            return docente;
        }

        // PUT: api/Docentes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocente(int id, Docente docente)
        {
            if (id != docente.Id)
            {
                return BadRequest();
            }

            _context.Entry(docente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DocenteExists(id))
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

        // POST: api/Docentes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CreateDocenteDto>> PostDocente(CreateDocenteDto docenteDto)
        {
            var docente = mapper.Map<Docente>(docenteDto);
            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDocente), new { id = docente.Id }, docente);
        }

        // DELETE: api/Docentes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocente(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null)
            {
                return NotFound();
            }

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
