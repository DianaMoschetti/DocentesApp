using DocentesApp.Shared.DTOs.Auth;
using AwesomeAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace DocentesApp.Tests.Integration
{
    // Seed de Identity fuera de Development/Testing: la contraseña del admin viene de Seed:AdminPassword
    // y los usuarios de prueba (editor, user) no se crean.
    public class SeedUsuariosIntegrationTests
    {
        [Fact]
        public void EnProduccion_SinSeedAdminPassword_LaApiNoArranca_YExplicaComoConfigurarla()
        {
            using var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(b => b.UseEnvironment("Production"));

            var act = () => factory.CreateClient();

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Seed:AdminPassword*Seed__AdminPassword*");
        }

        [Fact]
        public async Task EnProduccion_ConSeedAdminPassword_CreaSoloElAdmin_ConEsaContrasena()
        {
            using var factory = new CustomWebApplicationFactory()
                .WithWebHostBuilder(b =>
                {
                    b.UseEnvironment("Production");
                    b.UseSetting("Seed:AdminPassword", "Prod1234x");
                });
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

            (await Login(client, "admin@docentesapp.com", "Prod1234x")).Should().Be(HttpStatusCode.OK);
            (await Login(client, "admin@docentesapp.com", "Admin123!")).Should().Be(HttpStatusCode.Unauthorized);
            (await Login(client, "editor@docentesapp.com", "Editor123!")).Should().Be(HttpStatusCode.Unauthorized);
            (await Login(client, "user@docentesapp.com", "User123!")).Should().Be(HttpStatusCode.Unauthorized);
        }

        private static async Task<HttpStatusCode> Login(HttpClient client, string email, string password)
        {
            var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto { EmailOrUserName = email, Password = password });
            return response.StatusCode;
        }
    }
}
