using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Udbs;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocentesApp.Tests.Integration
{
    // Política por defecto de la API: todo endpoint requiere un usuario autenticado con rol Admin o User;
    // las escrituras y el registro de usuarios son solo Admin; el login es el único endpoint anónimo.
    public class AutorizacionIntegrationTests : IntegrationTestBase
    {
        public AutorizacionIntegrationTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/docentes")]
        [InlineData("/api/docentes/1")]
        [InlineData("/api/udbs")]
        [InlineData("/api/designaciones")]
        [InlineData("/api/designaciones/vigentes")]
        [InlineData("/api/asignaturas")]
        [InlineData("/api/cursos")]
        [InlineData("/api/puntosporcargo")]
        [InlineData("/api/reportes/planta/udb/1/por-asignatura")]
        public async Task Get_SinToken_Devuelve401(string url)
        {
            var response = await Client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData("/api/docentes")]
        [InlineData("/api/udbs")]
        [InlineData("/api/designaciones/vigentes")]
        [InlineData("/api/puntosporcargo")]
        public async Task Get_ConRolUser_Devuelve200(string url)
        {
            var user = await Auth.CreateUserClientAsync();

            var response = await user.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        }

        [Fact]
        public async Task Post_ConRolUser_Devuelve403()
        {
            var user = await Auth.CreateUserClientAsync();

            var response = await user.PostAsJsonAsync("/api/udbs", new CreateUdbDto { Nombre = "UDB no autorizada" });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Register_SinToken_Devuelve401()
        {
            var response = await Client.PostAsJsonAsync("/api/auth/register", NuevoUsuario());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_ConRolUser_Devuelve403()
        {
            var user = await Auth.CreateUserClientAsync();

            var response = await user.PostAsJsonAsync("/api/auth/register", NuevoUsuario());

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Register_ComoAdmin_Devuelve200ConLosDatosDelUsuarioYSinToken()
        {
            var admin = await Auth.CreateAdminClientAsync();
            var dto = NuevoUsuario();

            var response = await admin.PostAsJsonAsync("/api/auth/register", dto);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);

            var registrado = JsonSerializer.Deserialize<RegisteredUserDto>(body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            registrado!.Id.Should().NotBeNullOrWhiteSpace();
            registrado.Email.Should().Be(dto.Email);
            registrado.Role.Should().Be("User");

            // La respuesta no incluye ningún token
            using var json = JsonDocument.Parse(body);
            json.RootElement.EnumerateObject().Select(p => p.Name.ToLowerInvariant())
                .Should().NotContain("token");

            // El usuario creado se loguea y obtiene su propio token con rol User
            var login = await Auth.LoginAsync(dto.Email, dto.Password);
            login.Roles.Should().Contain("User");
        }

        // Swagger se sirve por middleware en Development: sigue accesible sin token (lo necesita NSwag)
        [Fact]
        public async Task Swagger_SinToken_Devuelve200()
        {
            var response = await Client.GetAsync("/swagger/v1/swagger.json");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private static RegisterDto NuevoUsuario()
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            return new RegisterDto
            {
                Email = $"nuevo-{unique}@test.com",
                UserName = $"nuevo{unique}",
                Password = "Nuevo123",
                Nombre = "Nuevo",
                Apellido = "Usuario"
            };
        }
    }
}
