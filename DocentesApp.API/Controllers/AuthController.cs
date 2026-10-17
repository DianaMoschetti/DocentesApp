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

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }

        // [Diana desde v4.0 OBSOLETO] el registro era público y devolvía el token del usuario nuevo:
        // cualquiera podía crearse una cuenta User y leer todos los datos. Ahora solo el Admin crea usuarios.
        //[HttpPost("register")]
        //[AllowAnonymous]
        //public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        //{
        [HttpPost("register")]
        // [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por políticas
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Politicas.EsAdministrador)]
        [ProducesResponseType(typeof(RegisteredUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RegisteredUserDto>> Register(RegisterDto dto)
        {
            var existingEmail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingEmail != null)
                return BadRequest("Ya existe un usuario con ese email.");

            var existingUserName = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUserName != null)
                return BadRequest("Ya existe un usuario con ese nombre de usuario.");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // [Diana desde v4.0 OBSOLETO] el rol "User" pasa a llamarse Consultante
            //await _userManager.AddToRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, Roles.Consultante);

            // [Diana desde v4.0 OBSOLETO] ya no se devuelve el token del usuario nuevo
            //var authResponse = await _tokenService.CreateTokenAsync(user);
            //
            //return Ok(authResponse);
            return Ok(new RegisteredUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                // [Diana desde v4.0 OBSOLETO] el rol "User" pasa a llamarse Consultante
                //Role = "User"
                Role = Roles.Consultante
            });
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

            // [Diana desde v4.0 OBSOLETO] devolvía un string plano, no ProblemDetails como el resto de la API
            //if (user == null)
            //    return Unauthorized("Credenciales inválidas.");
            if (user == null)
                throw new UnauthorizedAppException(MensajeCredencialesInvalidas);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            // [Diana desde v4.0 OBSOLETO] devolvía un string plano, no ProblemDetails como el resto de la API
            //if (!result.Succeeded)
            //    return Unauthorized("Credenciales inválidas.");
            if (!result.Succeeded)
                throw new UnauthorizedAppException(MensajeCredencialesInvalidas);

            // Recién con la contraseña correcta se informa que está inactivo (no revela qué usuarios existen)
            if (!user.Activo)
                throw new ForbiddenException(MensajeUsuarioInactivo);

            var authResponse = await _tokenService.CreateTokenAsync(user);
            return Ok(authResponse);
        }

        [HttpPost("assign-role")]
        // [Diana desde v4.0 OBSOLETO] roles Admin/User reemplazados por políticas
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Politicas.EsAdministrador)]
        public async Task<IActionResult> AssignRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound("Usuario no encontrado.");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return BadRequest("El rol no existe.");

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Rol '{roleName}' asignado a '{userName}'.");
        }
    }
}