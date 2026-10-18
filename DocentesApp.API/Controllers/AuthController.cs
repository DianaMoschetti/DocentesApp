using DocentesApp.API.Services;
using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUsuarioService _usuarioService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager,
            IUsuarioService usuarioService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _usuarioService = usuarioService;
        }

        public const string MensajeCredencialesInvalidas = "Credenciales inválidas.";
        public const string MensajeUsuarioInactivo = "Usuario inactivo. Contactá al administrador.";

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)] // credenciales inválidas
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]    // usuario inactivo
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            ApplicationUser? user = dto.EmailOrUserName.Contains("@")
                ? await _userManager.FindByEmailAsync(dto.EmailOrUserName)
                : await _userManager.FindByNameAsync(dto.EmailOrUserName);

            if (user == null)
                throw new UnauthorizedAppException(MensajeCredencialesInvalidas);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                throw new UnauthorizedAppException(MensajeCredencialesInvalidas);

            // Recién con la contraseña correcta se informa que está inactivo (no revela qué usuarios existen)
            if (!user.Activo)
                throw new ForbiddenException(MensajeUsuarioInactivo);

            var authResponse = await _tokenService.CreateTokenAsync(user);
            return Ok(authResponse);
        }

        // "Cambiar mi contraseña": cualquier usuario logueado (política por defecto, PuedeConsultar).
        // Cambiar la contraseña cambia el security stamp y el token actual deja de valer:
        // se devuelve un token nuevo para que Blazor renueve la sesión.
        [HttpPost("cambiar-password")]
        [Authorize]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // contraseña actual incorrecta o nueva inválida
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> CambiarMiPassword([FromBody] DocentesApp.Shared.DTOs.Usuarios.CambiarMiPasswordDto dto)
        {
            var userId = User.FindFirst(ValidacionTokenUsuario.ClaimUserId)?.Value
                ?? throw new UnauthorizedAppException(MensajeCredencialesInvalidas);

            return Ok(await _usuarioService.CambiarMiPasswordAsync(userId, dto));
        }
    }
}