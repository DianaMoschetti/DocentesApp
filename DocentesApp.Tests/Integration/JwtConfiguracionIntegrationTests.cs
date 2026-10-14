// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DocentesApp.Tests.Integration
{
    // La clave JWT no está en appsettings.json: en desarrollo viene de user-secrets y en los tests
    // del propio host de tests (CustomWebApplicationFactory.TestJwtKey).
    public class JwtConfiguracionIntegrationTests : IntegrationTestBase
    {
        public JwtConfiguracionIntegrationTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        // Los tokens del host de tests se firman con la clave de test y no con los user-secrets de la máquina
        [Fact]
        public async Task TokenDelHostDeTests_SeFirmaConLaClaveDeTest()
        {
            var login = await Auth.LoginAsync("admin@docentesapp.com", "Admin123!");

            var parametros = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CustomWebApplicationFactory.TestJwtKey))
            };

            var act = () => new JwtSecurityTokenHandler().ValidateToken(login.Token, parametros, out _);

            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void SinClaveJwt_LaApiNoArranca_YExplicaComoConfigurarla(string clave)
        {
            using var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(b => b.UseSetting("Jwt:Key", clave));

            var act = () => factory.CreateClient();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Falta la clave JWT (Jwt:Key)*dotnet user-secrets set*");
        }

        [Fact]
        public void ClaveJwtDemasiadoCorta_LaApiNoArranca()
        {
            using var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(b => b.UseSetting("Jwt:Key", "corta"));

            var act = () => factory.CreateClient();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*demasiado corta*32 bytes*");
        }
    }
}
