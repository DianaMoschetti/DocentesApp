using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DocentesApp.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var key = jwtSettings["Key"]
                ?? throw new InvalidOperationException("JWT Key no configurada.");

            var issuer = jwtSettings["Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer no configurado.");

            var audience = jwtSettings["Audience"]
                ?? throw new InvalidOperationException("JWT Audience no configurado.");

            var durationInMinutes = int.TryParse(jwtSettings["DurationInMinutes"], out var minutes)
                ? minutes
                : 60;

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new("userId", user.Id),
                new("userName", user.UserName ?? string.Empty),
                // Nombre para mostrar como autor en la auditoría (CurrentUserService): "Apellido, Nombre"; sin nombre y apellido, el userName
                new(CurrentUserService.ClaimNombre, NombreParaMostrar(user)),
                // Security stamp de Identity: cambia al desactivar, cambiar el rol o la contraseña.
                // La API lo compara en cada request (ValidacionTokenUsuario) para cortar los tokens viejos.
                new(ValidacionTokenUsuario.ClaimSecurityStamp, user.SecurityStamp ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(durationInMinutes);

            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiration = expiration,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Roles = roles
            };
        }

        // [Diana desde v4.0 OBSOLETO] formato "Nombre Apellido"; la app muestra a las personas como "Apellido, Nombre"
        //// "Nombre Apellido"; si no tiene ninguno de los dos, el userName (o el email)
        //public static string NombreParaMostrar(ApplicationUser user)
        //{
        //    var nombre = $"{user.Nombre} {user.Apellido}".Trim();
        //    return nombre.Length > 0 ? nombre : user.UserName ?? user.Email ?? string.Empty;
        //}

        // "Apellido, Nombre"; con uno solo de los dos, ese; sin ninguno, el userName (o el email)
        public static string NombreParaMostrar(ApplicationUser user)
        {
            var partes = new[] { user.Apellido, user.Nombre }.Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => p!.Trim());
            var nombre = string.Join(", ", partes);
            return nombre.Length > 0 ? nombre : user.UserName ?? user.Email ?? string.Empty;
        }
    }
}