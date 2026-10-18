using System.Security.Claims;
using DocentesApp.API.Services;
using DocentesApp.Data.Identity;
using AwesomeAssertions;
using Microsoft.AspNetCore.Http;

namespace DocentesApp.Tests.Auditoria
{
    // CurrentUserService: usuario actual desde los claims del token, sin consultar la base
    public class CurrentUserServiceTests
    {
        private static CurrentUserService Servicio(HttpContext? httpContext) =>
            new(new HttpContextAccessor { HttpContext = httpContext });

        private static HttpContext ContextoConClaims(bool autenticado, params Claim[] claims) =>
            new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, autenticado ? "Bearer" : null)) };

        [Fact]
        public void ConToken_DevuelveIdNombreYEmail()
        {
            var servicio = Servicio(ContextoConClaims(true,
                new Claim("userId", "u-1"),
                new Claim("userName", "eva"),
                new Claim(CurrentUserService.ClaimNombre, "Editora, Eva"),
                new Claim(ClaimTypes.Email, "eva@test.com")));

            servicio.UserId.Should().Be("u-1");
            servicio.Nombre.Should().Be("Editora, Eva");
            servicio.Email.Should().Be("eva@test.com");
        }

        [Fact]
        public void TokenViejoSinClaimNombre_UsaElUserName()
        {
            var servicio = Servicio(ContextoConClaims(true, new Claim("userId", "u-1"), new Claim("userName", "eva")));

            servicio.Nombre.Should().Be("eva");
        }

        [Fact]
        public void SinHttpContext_TodoNull()
        {
            var servicio = Servicio(null);

            servicio.UserId.Should().BeNull();
            servicio.Nombre.Should().BeNull();
            servicio.Email.Should().BeNull();
        }

        // Formato del claim "nombre" que emite TokenService
        [Theory]
        [InlineData("Eva", "Editora", "eva", "Editora, Eva")]
        [InlineData(null, "Editora", "eva", "Editora")]
        [InlineData("Eva", null, "eva", "Eva")]
        [InlineData(null, " ", "eva", "eva")]
        public void NombreParaMostrar_ApellidoComaNombre_ConFallbacks(string? nombre, string? apellido, string userName, string esperado)
        {
            var user = new ApplicationUser { Nombre = nombre, Apellido = apellido, UserName = userName, Email = "eva@test.com" };

            TokenService.NombreParaMostrar(user).Should().Be(esperado);
        }

        [Fact]
        public void UsuarioNoAutenticado_TodoNull()
        {
            var servicio = Servicio(ContextoConClaims(false, new Claim("userId", "u-1")));

            servicio.UserId.Should().BeNull();
            servicio.Nombre.Should().BeNull();
        }
    }
}
