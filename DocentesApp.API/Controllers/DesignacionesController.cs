using DocentesApp.Data.Context;
using DocentesApp.Shared.DTOs.Designaciones;
using Microsoft.AspNetCore.Mvc;


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
        public ActionResult<IEnumerable<DesignacionDto>> GetDesignaciones()
        {
            return StatusCode(501); // Not Implemented por ahora
        }

        [HttpGet("{id}")]
        public ActionResult<DesignacionDto> GetDesignacion(int id)
        {
            return StatusCode(501);
        }

        [HttpPost]
        public ActionResult<DesignacionDto> PostDesignacion(DesignacionDto body)
        {
            return StatusCode(501);
        }

        [HttpPut("{id}")]
        public IActionResult PutDesignacion(int id, DesignacionDto body)
        {
            return StatusCode(501);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDesignacion(int id)
        {
            return StatusCode(501);
        }
    }
}