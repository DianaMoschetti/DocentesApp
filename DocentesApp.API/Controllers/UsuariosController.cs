using DocentesApp.API.Services;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocentesApp.API.Controllers
{
    // Gestión de usuarios: solo Administrador. Reemplaza a auth/register y auth/assign-role.
    // "Cambiar mi contraseña" está en AuthController: las políticas de clase y de acción se suman (AND),
    // así que no puede vivir acá debajo de EsAdministrador.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.EsAdministrador)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Id del administrador que hace la operación (para las reglas "no a uno mismo")
        private string ActorId => User.FindFirst(ValidacionTokenUsuario.ClaimUserId)?.Value ?? string.Empty;

        #region GET

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            return Ok(await _usuarioService.GetAllAsync());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(string id)
        {
            return Ok(await _usuarioService.GetByIdAsync(id));
        }

        #endregion

        #region Create / Update

        [HttpPost]
        [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // datos inválidos, contraseña débil
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]   // email o nombre de usuario repetido
        public async Task<ActionResult<UsuarioDto>> PostUsuario([FromBody] CreateUsuarioDto dto)
        {
            var usuario = await _usuarioService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        // Solo Nombre y Apellido: el email no se edita
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUsuario(string id, [FromBody] UpdateUsuarioDto dto)
        {
            await _usuarioService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpPut("{id}/rol")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // su propio rol / último administrador
        public async Task<IActionResult> CambiarRol(string id, [FromBody] CambiarRolDto dto)
        {
            await _usuarioService.CambiarRolAsync(id, dto.Rol, ActorId);
            return NoContent();
        }

        #endregion

        #region Activar / desactivar / contraseña

        [HttpPatch("{id}/desactivar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] // a sí mismo / último administrador
        public async Task<IActionResult> Desactivar(string id)
        {
            await _usuarioService.DesactivarAsync(id, ActorId);
            return NoContent();
        }

        [HttpPatch("{id}/reactivar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reactivar(string id)
        {
            await _usuarioService.ReactivarAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/reset-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordDto dto)
        {
            await _usuarioService.ResetPasswordAsync(id, dto.PasswordNueva);
            return NoContent();
        }

        #endregion
    }
}
