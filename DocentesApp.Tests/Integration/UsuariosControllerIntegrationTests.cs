using DocentesApp.API.Controllers;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Usuarios;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace DocentesApp.Tests.Integration
{
    // Gestión de usuarios (api/usuarios, solo Administrador) y "Cambiar mi contraseña" (api/auth/cambiar-password).
    // Cada test crea sus propios usuarios: el admin del seed se comparte entre tests y no se modifica.
    public class UsuariosControllerIntegrationTests : IntegrationTestBase
    {
        private const string Password = "Usuario123";
        private static readonly JsonSerializerOptions Web = new(JsonSerializerDefaults.Web);

        public UsuariosControllerIntegrationTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        #region Listado y alta

        [Fact]
        public async Task GetUsuarios_DevuelveLosUsuariosConRolYEstado()
        {
            var creado = await Auth.RegisterUserAsync(password: Password, rol: Roles.Editor);
            var admin = await Auth.CreateAdminClientAsync();

            var usuarios = await admin.GetFromJsonAsync<List<UsuarioDto>>("/api/usuarios");

            usuarios.Should().Contain(u => u.Email == "admin@docentesapp.com" && u.Rol == Roles.Administrador && u.Activo);
            usuarios.Should().ContainSingle(u => u.Id == creado.Id)
                .Which.Should().BeEquivalentTo(new { creado.Email, Rol = Roles.Editor, Activo = true, Nombre = "Test", Apellido = "User" });
        }

        [Fact]
        public async Task GetUsuario_QueNoExiste_Devuelve404ProblemDetails()
        {
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.GetAsync("/api/usuarios/no-existe");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            (await Problem(response)).Detail.Should().Be("No se encontró el usuario con ID no-existe.");
        }

        [Theory]
        [InlineData(Roles.Administrador)]
        [InlineData(Roles.Editor)]
        [InlineData(Roles.Consultante)]
        public async Task CrearUsuario_ConRol_ElLoginDevuelveEseRol(string rol)
        {
            var creado = await Auth.RegisterUserAsync(password: Password, rol: rol);

            creado.Rol.Should().Be(rol);
            var login = await Auth.LoginAsync(creado.Email, Password);
            login.Roles.Should().ContainSingle().Which.Should().Be(rol);
        }

        [Fact]
        public async Task CrearUsuario_ConRolInvalido_Devuelve400()
        {
            var dto = NuevoUsuario("NoExiste");

            var response = await (await Auth.CreateAdminClientAsync()).PostAsJsonAsync("/api/usuarios", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await response.Content.ReadAsStringAsync()).Should().Contain("El rol debe ser uno de");
        }

        [Fact]
        public async Task CrearUsuario_EmailRepetido_Devuelve409ProblemDetails()
        {
            var existente = await Auth.RegisterUserAsync(password: Password);
            var dto = NuevoUsuario(Roles.Consultante);
            dto.Email = existente.Email;

            var response = await (await Auth.CreateAdminClientAsync()).PostAsJsonAsync("/api/usuarios", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
            (await Problem(response)).Detail.Should().Be($"Ya existe un usuario con el email {existente.Email}.");
        }

        [Fact]
        public async Task CrearUsuario_UserNameRepetido_Devuelve409ProblemDetails()
        {
            var existente = await Auth.RegisterUserAsync(password: Password);
            var dto = NuevoUsuario(Roles.Consultante);
            dto.UserName = existente.UserName;

            var response = await (await Auth.CreateAdminClientAsync()).PostAsJsonAsync("/api/usuarios", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            (await Problem(response)).Detail.Should().Be($"Ya existe un usuario con el nombre de usuario {existente.UserName}.");
        }

        [Fact]
        public async Task CrearUsuario_PasswordDebil_Devuelve400ConMensajeEnCastellano()
        {
            var dto = NuevoUsuario(Roles.Consultante);
            dto.Password = "abc";

            var response = await (await Auth.CreateAdminClientAsync()).PostAsJsonAsync("/api/usuarios", dto);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            body.Should().Contain("al menos 6 caracteres").And.Contain("al menos un número");
        }

        #endregion

        #region Edición y rol

        [Fact]
        public async Task EditarUsuario_CambiaNombreYApellido_NoElEmail()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.PutAsJsonAsync($"/api/usuarios/{creado.Id}",
                new UpdateUsuarioDto { Nombre = "Nuevo nombre", Apellido = "Nuevo apellido" });

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var usuario = await admin.GetFromJsonAsync<UsuarioDto>($"/api/usuarios/{creado.Id}");
            usuario!.Nombre.Should().Be("Nuevo nombre");
            usuario.Apellido.Should().Be("Nuevo apellido");
            usuario.Email.Should().Be(creado.Email);
        }

        [Fact]
        public async Task CambiarRol_CortaElTokenViejo_YElNuevoLoginTieneElRolNuevo()
        {
            var creado = await Auth.RegisterUserAsync(password: Password, rol: Roles.Editor);
            var editor = await Auth.CreateClientForAsync(creado.Email, Password);
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.PutAsJsonAsync($"/api/usuarios/{creado.Id}/rol", new CambiarRolDto { Rol = Roles.Consultante });

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            (await editor.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var login = await Auth.LoginAsync(creado.Email, Password);
            login.Roles.Should().ContainSingle().Which.Should().Be(Roles.Consultante);
        }

        [Fact]
        public async Task CambiarRol_MismoRol_NoCortaLaSesion()
        {
            var creado = await Auth.RegisterUserAsync(password: Password, rol: Roles.Editor);
            var editor = await Auth.CreateClientForAsync(creado.Email, Password);
            var admin = await Auth.CreateAdminClientAsync();

            (await admin.PutAsJsonAsync($"/api/usuarios/{creado.Id}/rol", new CambiarRolDto { Rol = Roles.Editor }))
                .StatusCode.Should().Be(HttpStatusCode.NoContent);

            (await editor.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CambiarSuPropioRol_Devuelve409()
        {
            // un admin propio del test, para no tocar al admin del seed
            var creado = await Auth.RegisterUserAsync(password: Password, rol: Roles.Administrador);
            var yo = await Auth.CreateClientForAsync(creado.Email, Password);

            var response = await yo.PutAsJsonAsync($"/api/usuarios/{creado.Id}/rol", new CambiarRolDto { Rol = Roles.Editor });

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            (await Problem(response)).Detail.Should().Be("No podés cambiar tu propio rol.");
        }

        #endregion

        #region Desactivar / reactivar

        [Fact]
        public async Task Desactivar_ElLoginDevuelve403_YElTokenAbiertoDevuelve401()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var sesionAbierta = await Auth.CreateClientForAsync(creado.Email, Password);
            var admin = await Auth.CreateAdminClientAsync();

            (await admin.PatchAsync($"/api/usuarios/{creado.Id}/desactivar", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);

            (await sesionAbierta.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var login = await Login(creado.Email, Password);
            login.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            (await Problem(login)).Detail.Should().Be(AuthController.MensajeUsuarioInactivo);

            var usuario = await admin.GetFromJsonAsync<UsuarioDto>($"/api/usuarios/{creado.Id}");
            usuario!.Activo.Should().BeFalse();
        }

        [Fact]
        public async Task Reactivar_VuelveAPoderLoguearse()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var admin = await Auth.CreateAdminClientAsync();
            await admin.PatchAsync($"/api/usuarios/{creado.Id}/desactivar", null);

            (await admin.PatchAsync($"/api/usuarios/{creado.Id}/reactivar", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);

            (await Login(creado.Email, Password)).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DesactivarseASiMismo_Devuelve409()
        {
            var creado = await Auth.RegisterUserAsync(password: Password, rol: Roles.Administrador);
            var yo = await Auth.CreateClientForAsync(creado.Email, Password);

            var response = await yo.PatchAsync($"/api/usuarios/{creado.Id}/desactivar", null);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            (await Problem(response)).Detail.Should().Be("No podés desactivarte a vos mismo.");
        }

        #endregion

        #region Contraseñas

        [Fact]
        public async Task ResetPassword_LaViejaNoSirve_LaNuevaSi_YElTokenViejoDevuelve401()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var sesionAbierta = await Auth.CreateClientForAsync(creado.Email, Password);
            var admin = await Auth.CreateAdminClientAsync();

            var response = await admin.PostAsJsonAsync($"/api/usuarios/{creado.Id}/reset-password",
                new ResetPasswordDto { PasswordNueva = "Reseteada1" });

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            (await Login(creado.Email, Password)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            (await Login(creado.Email, "Reseteada1")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await sesionAbierta.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CambiarMiPassword_DevuelveTokenNuevo_YElViejoDevuelve401()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var yo = await Auth.CreateClientForAsync(creado.Email, Password);

            var response = await yo.PostAsJsonAsync("/api/auth/cambiar-password",
                new CambiarMiPasswordDto { PasswordActual = Password, PasswordNueva = "Cambiada1" });
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);
            var nuevo = JsonSerializer.Deserialize<AuthResponseDto>(body, Web)!;
            nuevo.Roles.Should().ContainSingle().Which.Should().Be(Roles.Consultante);

            (await yo.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            (await Auth.CreateAuthenticatedClient(nuevo.Token).GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.OK);
            (await Login(creado.Email, "Cambiada1")).StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CambiarMiPassword_ConLaActualIncorrecta_Devuelve400()
        {
            var creado = await Auth.RegisterUserAsync(password: Password);
            var yo = await Auth.CreateClientForAsync(creado.Email, Password);

            var response = await yo.PostAsJsonAsync("/api/auth/cambiar-password",
                new CambiarMiPasswordDto { PasswordActual = "Incorrecta1", PasswordNueva = "Cambiada1" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            (await Problem(response)).Detail.Should().Be("La contraseña actual no es correcta.");
            (await yo.GetAsync("/api/docentes")).StatusCode.Should().Be(HttpStatusCode.OK); // la sesión sigue
        }

        [Fact]
        public async Task CambiarMiPassword_SinToken_Devuelve401()
        {
            var response = await Client.PostAsJsonAsync("/api/auth/cambiar-password",
                new CambiarMiPasswordDto { PasswordActual = Password, PasswordNueva = "Cambiada1" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        private static CreateUsuarioDto NuevoUsuario(string rol)
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            return new CreateUsuarioDto
            {
                Email = $"usuario-{unique}@test.com",
                UserName = $"usuario{unique}",
                Password = Password,
                Nombre = "Nuevo",
                Apellido = "Usuario",
                Rol = rol
            };
        }

        private Task<HttpResponseMessage> Login(string email, string password) =>
            Client.PostAsJsonAsync("/api/auth/login", new LoginDto { EmailOrUserName = email, Password = password });

        private static async Task<ProblemDetails> Problem(HttpResponseMessage response) =>
            JsonSerializer.Deserialize<ProblemDetails>(await response.Content.ReadAsStringAsync(), Web)!;
    }
}
