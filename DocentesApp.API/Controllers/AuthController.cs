using DocentesApp.API.Services;
using DocentesApp.Application.DTOs.Auth;
using DocentesApp.Data.Identity;
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
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

            await _userManager.AddToRoleAsync(user, "User");

            var authResponse = await _tokenService.CreateTokenAsync(user);

            return Ok(authResponse);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            ApplicationUser? user = dto.EmailOrUserName.Contains("@")
                ? await _userManager.FindByEmailAsync(dto.EmailOrUserName)
                : await _userManager.FindByNameAsync(dto.EmailOrUserName);

            if (user == null)
                return Unauthorized("Credenciales inválidas.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Credenciales inválidas.");

            var authResponse = await _tokenService.CreateTokenAsync(user);
            return Ok(authResponse);
        }

        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
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