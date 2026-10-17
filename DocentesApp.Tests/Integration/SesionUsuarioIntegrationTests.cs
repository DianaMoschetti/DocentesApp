using DocentesApp.API.Controllers;
using DocentesApp.API.Services;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.DTOs.Auth;
using AwesomeAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DocentesApp.Tests.Integration
{
    // Login con ProblemDetails, usuario inactivo y corte de tokens ya emitidos (security stamp).
    // Cada test usa un usuario propio: los usuarios del seed se comparten entre tests y no se tocan.
    public class SesionUsuarioIntegrationTests : IntegrationTestBase
    {
        private const string Password = "Sesion123";

        public SesionUsuarioIntegrationTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        #region Login

        [Fact]
        public async Task Login_CredencialesInvalidas_Devuelve401ProblemDetails()
        {
            var response = await Login("noexiste@test.com", "WrongPassword");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
            (await Problem(response)).Detail.Should().Be(AuthController.MensajeCredencialesInvalidas);
        }

        [Fact]
        public async Task Login_UsuarioInactivo_ConPasswordCorrecta_Devuelve403ConMensaje()
        {
            var email = await CrearUsuarioAsync();
            await ModificarUsuarioAsync(email, u => u.Activo = false);

            var response = await Login(email, Password);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            (await Problem(response)).Detail.Should().Be("Usuario inactivo. Contactá al administrador.");
        }

        [Fact]
        public async Task Login_UsuarioInactivo_ConPasswordIncorrecta_Devuelve401_SinRevelarQueEstaInactivo()
        {
            var email = await CrearUsuarioAsync();
            await ModificarUsuarioAsync(email, u => u.Activo = false);

            var response = await Login(email, "Incorrecta1");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            (await Problem(response)).Detail.Should().Be(AuthController.MensajeCredencialesInvalidas);
        }

        [Fact]
        public async Task Login_DevuelveTokenConSecurityStamp()
        {
            var email = await CrearUsuarioAsync();

            var login = await Auth.LoginAsync(email, Password);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(login.Token);
            jwt.Claims.Should().Contain(c => c.Type == ValidacionTokenUsuario.ClaimSecurityStamp && c.Value != "");
        }

        #endregion

        #region Tokens ya emitidos

        [Fact]
        public async Task TokenVigente_DeUsuarioActivo_Devuelve200()
        {
            var email = await CrearUsuarioAsync();
            var client = await Auth.CreateClientForAsync(email, Password);

            var response = await client.GetAsync("/api/docentes");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task TokenDeUsuarioDesactivado_Devuelve401_AunqueNoHayaVencido()
        {
            var email = await CrearUsuarioAsync();
            var client = await Auth.CreateClientForAsync(email, Password);

            await ModificarUsuarioAsync(email, u => u.Activo = false);

            (await client.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task TokenConSecurityStampViejo_Devuelve401()
        {
            var email = await CrearUsuarioAsync();
            var client = await Auth.CreateClientForAsync(email, Password);

            // lo que harán el cambio de rol y los cambios de contraseña (etapa 4)
            using (var scope = Factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await userManager.FindByEmailAsync(email);
                await userManager.UpdateSecurityStampAsync(user!);
            }

            (await client.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            // con un login nuevo vuelve a funcionar
            var nuevo = await Auth.CreateClientForAsync(email, Password);
            (await nuevo.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task TokenSinSecurityStamp_Devuelve401()
        {
            // Token bien firmado y vigente pero sin "sstamp", como los emitidos antes de este cambio
            var email = await CrearUsuarioAsync();
            string userId;
            using (var scope = Factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                userId = (await userManager.FindByEmailAsync(email))!.Id;
            }

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CustomWebApplicationFactory.TestJwtKey)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                issuer: "DocentesApp",
                audience: "DocentesAppUsers",
                claims: new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(ValidacionTokenUsuario.ClaimUserId, userId),
                    new Claim(ClaimTypes.Role, DocentesApp.Shared.Auth.Roles.Consultante)
                },
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credenciales));

            var client = Auth.CreateAuthenticatedClient(token);

            (await client.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        private async Task<string> CrearUsuarioAsync()
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            var email = $"sesion-{unique}@test.com";
            await Auth.RegisterUserAsync(email, $"sesion{unique}", Password);
            return email;
        }

        private async Task ModificarUsuarioAsync(string email, Action<ApplicationUser> cambio)
        {
            using var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(email);
            cambio(user!);
            var result = await userManager.UpdateAsync(user!);
            result.Succeeded.Should().BeTrue();
        }

        private Task<HttpResponseMessage> Login(string email, string password) =>
            Client.PostAsJsonAsync("/api/auth/login", new LoginDto { EmailOrUserName = email, Password = password });

        private static async Task<ProblemDetails> Problem(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProblemDetails>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
        }
    }
}
