using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Udbs;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocentesApp.Tests.Integration
{
    // [Diana desde v4.0 OBSOLETO] roles Admin/User
    // Política por defecto de la API: todo endpoint requiere un usuario autenticado con rol Admin o User;
    // las escrituras y el registro de usuarios son solo Admin; el login es el único endpoint anónimo.
    // Ahora: lecturas = cualquier rol (PuedeConsultar), escrituras = Administrador o Editor (PuedeEditar),
    // gestión de usuarios = Administrador (EsAdministrador); el login sigue siendo el único endpoint anónimo.
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
            // [Diana desde v4.0 OBSOLETO] el rol "User" pasa a llamarse Consultante
            //registrado.Role.Should().Be("User");
            registrado.Role.Should().Be(Roles.Consultante);

            // La respuesta no incluye ningún token
            using var json = JsonDocument.Parse(body);
            json.RootElement.EnumerateObject().Select(p => p.Name.ToLowerInvariant())
                .Should().NotContain("token");

            // El usuario creado se loguea y obtiene su propio token con rol User
            var login = await Auth.LoginAsync(dto.Email, dto.Password);
            // [Diana desde v4.0 OBSOLETO] el rol "User" pasa a llamarse Consultante
            //login.Roles.Should().Contain("User");
            login.Roles.Should().ContainSingle().Which.Should().Be(Roles.Consultante);
        }

        #region Matriz de roles (políticas PuedeConsultar / PuedeEditar / EsAdministrador)

        public static IEnumerable<object[]> Lecturas()
        {
            var urls = new[]
            {
                "/api/docentes", "/api/designaciones", "/api/designaciones/vigentes", "/api/udbs",
                "/api/puntosporcargo", "/api/asignaturas", "/api/cursos", "/api/reportes/planta/udb/1/por-asignatura"
            };
            foreach (var rol in Roles.Todos)
                foreach (var url in urls)
                    yield return new object[] { rol, url };
        }

        // Escrituras con body vacío o Id inexistente: si la política deja pasar, la API responde 400/404 y no crea nada
        private static readonly (string Metodo, string Url)[] Escrituras =
        {
            ("POST", "/api/docentes"),
            ("POST", "/api/udbs"),
            ("POST", "/api/asignaturas"),
            ("POST", "/api/cursos"),
            ("POST", "/api/puntosporcargo"),
            ("PUT", "/api/designaciones/999999"),
            ("PATCH", "/api/designaciones/999999/cerrar"),
            ("DELETE", "/api/docentes/999999")
        };

        public static IEnumerable<object[]> EscriturasPorRol()
        {
            foreach (var rol in Roles.Todos)
                foreach (var (metodo, url) in Escrituras)
                    yield return new object[] { rol, metodo, url };
        }

        [Theory]
        [MemberData(nameof(Lecturas))]
        public async Task Lectura_CualquierRol_Devuelve200(string rol, string url)
        {
            var client = await ClientePorRolAsync(rol);

            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, $"{rol} GET {url}: {body}");
        }

        [Theory]
        [MemberData(nameof(EscriturasPorRol))]
        public async Task Escritura_SoloAdministradorYEditor(string rol, string metodo, string url)
        {
            var client = await ClientePorRolAsync(rol);

            var request = new HttpRequestMessage(new HttpMethod(metodo), url);
            if (metodo != "DELETE")
                request.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (rol == Roles.Consultante)
                response.StatusCode.Should().Be(HttpStatusCode.Forbidden, $"{rol} {metodo} {url}");
            else
                response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden, $"{rol} {metodo} {url}: {body}")
                    .And.NotBe(HttpStatusCode.Unauthorized);
        }

        // Gestión de usuarios: solo Administrador (hoy register; en la etapa 4 pasa a api/usuarios)
        [Theory]
        [InlineData(Roles.Consultante)]
        [InlineData(Roles.Editor)]
        public async Task Register_SinRolAdministrador_Devuelve403(string rol)
        {
            var client = await ClientePorRolAsync(rol);

            var response = await client.PostAsJsonAsync("/api/auth/register", NuevoUsuario());

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Theory]
        [InlineData("admin@docentesapp.com", "Admin123!", Roles.Administrador)]
        [InlineData("editor@docentesapp.com", "Editor123!", Roles.Editor)]
        [InlineData("user@docentesapp.com", "User123!", Roles.Consultante)]
        public async Task UsuariosDelSeed_TienenUnSoloRol(string email, string password, string rol)
        {
            var login = await Auth.LoginAsync(email, password);

            login.Roles.Should().ContainSingle().Which.Should().Be(rol);
        }

        private Task<HttpClient> ClientePorRolAsync(string rol) => rol switch
        {
            Roles.Administrador => Auth.CreateAdminClientAsync(),
            Roles.Editor => Auth.CreateEditorClientAsync(),
            Roles.Consultante => Auth.CreateConsultanteClientAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(rol), rol, null)
        };

        #endregion

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
