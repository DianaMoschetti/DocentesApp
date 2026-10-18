using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using DocentesApp.Data.Context;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Shared.DTOs.Usuarios;
using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Integration
{
    // Auditoría de usuarios (etapa 4b): columnas por el interceptor; rol y contraseña como eventos explícitos.
    // Ninguna fila guarda la contraseña, el hash ni los demás campos técnicos de Identity.
    public class AuditoriaUsuariosIntegrationTests : IntegrationTestBase
    {
        private const string Password = "Inicial123";

        private static readonly string[] CamposTecnicos =
        {
            "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "AccessFailedCount",
            "LockoutEnd", "LockoutEnabled", "NormalizedEmail", "NormalizedUserName"
        };

        public AuditoriaUsuariosIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        #region Helpers

        private List<HistorialCambio> Historial(Func<HistorialCambio, bool>? filtro = null)
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var filas = db.HistorialCambios.Include(h => h.Detalles).AsNoTracking().OrderBy(h => h.Id).ToList();
            return filtro == null ? filas : filas.Where(filtro).ToList();
        }

        private List<HistorialCambio> HistorialDelUsuario(string usuarioId, long desde = 0) =>
            Historial(h => h.Entidad == "Usuario" && h.EntidadId == usuarioId && h.Id > desde);

        private long UltimoId() => Historial().Select(h => h.Id).DefaultIfEmpty(0).Max();

        private static string UserIdDelToken(string token) =>
            new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.Single(c => c.Type == "userId").Value;

        private async Task<(HttpClient Admin, string AdminId)> AdminAsync()
        {
            var login = await Auth.LoginAsync("admin@docentesapp.com", "Admin123!");
            return (Auth.CreateAuthenticatedClient(login.Token), UserIdDelToken(login.Token));
        }

        private async Task<UsuarioDto> CrearUsuarioAsync(string rol = Roles.Consultante) =>
            await Auth.RegisterUserAsync(password: Password, rol: rol);

        private static async Task EsperarExitoAsync(HttpResponseMessage response) =>
            response.IsSuccessStatusCode.Should().BeTrue(await response.Content.ReadAsStringAsync());

        #endregion

        [Fact]
        public async Task Login_ExitosoOFallido_NoRegistraFilas()
        {
            var usuario = await CrearUsuarioAsync();
            var desde = UltimoId();

            await Auth.LoginAsync(usuario.UserName, Password);
            var fallido = await Client.PostAsJsonAsync("/api/auth/login",
                new LoginDto { EmailOrUserName = usuario.UserName, Password = "Incorrecta1" }); // AccessFailedCount: campo técnico
            fallido.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            Historial(h => h.Id > desde).Should().BeEmpty();
        }

        [Fact]
        public async Task CrearUsuario_AltaYRol_MismaOperacion_SinDatosDeContrasenia()
        {
            var (_, adminId) = await AdminAsync();

            var usuario = await CrearUsuarioAsync(Roles.Editor);

            var filas = HistorialDelUsuario(usuario.Id);
            filas.Should().HaveCount(2);
            filas.Select(f => f.OperacionId).Distinct().Should().ContainSingle();
            filas.Should().OnlyContain(f => f.UsuarioId == adminId);

            var alta = filas.Single(f => f.Accion == AccionAuditoria.Alta);
            alta.EntidadDescripcion.Should().Be($"User, Test ({usuario.UserName})");
            alta.Detalles.Select(d => d.Propiedad).Should().Contain(new[] { "UserName", "Email", "Nombre", "Apellido", "Activo" })
                .And.NotContain(CamposTecnicos);

            var rol = filas.Single(f => f.Accion == AccionAuditoria.Modificacion);
            rol.Detalles.Should().ContainSingle(d => d.Propiedad == "Rol" && d.ValorAnterior == null && d.ValorNuevo == Roles.Editor);

            Historial().SelectMany(h => h.Detalles).Should().NotContain(d =>
                CamposTecnicos.Contains(d.Propiedad) || d.ValorAnterior == Password || d.ValorNuevo == Password);
        }

        [Fact]
        public async Task DesactivarYReactivar_RegistranLaModificacionDeActivo()
        {
            var (admin, _) = await AdminAsync();
            var usuario = await CrearUsuarioAsync();
            var desde = UltimoId();

            await EsperarExitoAsync(await admin.PatchAsync($"/api/usuarios/{usuario.Id}/desactivar", null));
            await EsperarExitoAsync(await admin.PatchAsync($"/api/usuarios/{usuario.Id}/reactivar", null));

            var filas = HistorialDelUsuario(usuario.Id, desde);
            filas.Should().HaveCount(2).And.OnlyContain(f => f.Accion == AccionAuditoria.Modificacion);
            filas[0].Detalles.Should().ContainSingle()
                .Which.Should().Match<HistorialCambioDetalle>(d => d.Propiedad == "Activo" && d.ValorAnterior == "true" && d.ValorNuevo == "false");
            filas[1].Detalles.Should().ContainSingle()
                .Which.Should().Match<HistorialCambioDetalle>(d => d.Propiedad == "Activo" && d.ValorAnterior == "false" && d.ValorNuevo == "true");
        }

        [Fact]
        public async Task CambiarRol_RegistraSoloElEventoDeRol_NadaPorElSecurityStamp()
        {
            var (admin, adminId) = await AdminAsync();
            var usuario = await CrearUsuarioAsync(Roles.Consultante);
            var desde = UltimoId();

            await EsperarExitoAsync(await admin.PutAsJsonAsync($"/api/usuarios/{usuario.Id}/rol", new CambiarRolDto { Rol = Roles.Editor }));

            var fila = HistorialDelUsuario(usuario.Id, desde).Should().ContainSingle().Subject;
            fila.Accion.Should().Be(AccionAuditoria.Modificacion);
            fila.UsuarioId.Should().Be(adminId);
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Rol" && d.ValorAnterior == Roles.Consultante && d.ValorNuevo == Roles.Editor);
        }

        [Fact]
        public async Task ResetDeContraseniaPorElAdmin_RegistraContraseniaSinValores()
        {
            var (admin, adminId) = await AdminAsync();
            var usuario = await CrearUsuarioAsync();
            var desde = UltimoId();

            await EsperarExitoAsync(await admin.PostAsJsonAsync($"/api/usuarios/{usuario.Id}/reset-password",
                new ResetPasswordDto { PasswordNueva = "Reseteada123" }));

            var fila = HistorialDelUsuario(usuario.Id, desde).Should().ContainSingle().Subject;
            fila.UsuarioId.Should().Be(adminId);
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Contraseña" && d.ValorAnterior == null && d.ValorNuevo == null);
            await Auth.LoginAsync(usuario.UserName, "Reseteada123"); // el cambio se confirmó junto con el evento
        }

        [Fact]
        public async Task CambiarMiContrasenia_RegistraContraseniaConElPropioUsuarioComoAutor()
        {
            var usuario = await CrearUsuarioAsync();
            var login = await Auth.LoginAsync(usuario.UserName, Password);
            var cliente = Auth.CreateAuthenticatedClient(login.Token);
            var desde = UltimoId();

            await EsperarExitoAsync(await cliente.PostAsJsonAsync("/api/auth/cambiar-password",
                new CambiarMiPasswordDto { PasswordActual = Password, PasswordNueva = "Cambiada123" }));

            var fila = HistorialDelUsuario(usuario.Id, desde).Should().ContainSingle().Subject;
            fila.UsuarioId.Should().Be(usuario.Id);
            fila.UsuarioNombre.Should().Be("User, Test");
            fila.Detalles.Should().ContainSingle(d => d.Propiedad == "Contraseña" && d.ValorAnterior == null && d.ValorNuevo == null);
        }

        [Fact]
        public async Task CambiarMiContraseniaConLaActualIncorrecta_NoRegistraNada()
        {
            var usuario = await CrearUsuarioAsync();
            var login = await Auth.LoginAsync(usuario.UserName, Password);
            var cliente = Auth.CreateAuthenticatedClient(login.Token);
            var desde = UltimoId();

            var response = await cliente.PostAsJsonAsync("/api/auth/cambiar-password",
                new CambiarMiPasswordDto { PasswordActual = "Incorrecta1", PasswordNueva = "Cambiada123" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            HistorialDelUsuario(usuario.Id, desde).Should().BeEmpty();
        }

        [Fact]
        public async Task EditarNombreYApellido_RegistraModificacionConValoresAnterioresYNuevos()
        {
            var (admin, _) = await AdminAsync();
            var usuario = await CrearUsuarioAsync();
            var desde = UltimoId();

            await EsperarExitoAsync(await admin.PutAsJsonAsync($"/api/usuarios/{usuario.Id}",
                new UpdateUsuarioDto { Nombre = "Ana", Apellido = "Gomez" }));

            var fila = HistorialDelUsuario(usuario.Id, desde).Should().ContainSingle().Subject;
            fila.Accion.Should().Be(AccionAuditoria.Modificacion);
            fila.Detalles.Select(d => (d.Propiedad, d.ValorAnterior, d.ValorNuevo)).Should().BeEquivalentTo(new[]
            {
                ("Nombre", (string?)"Test", (string?)"Ana"),
                ("Apellido", (string?)"User", (string?)"Gomez")
            });
        }

        [Theory]
        [InlineData(Roles.Editor)]
        [InlineData(Roles.Consultante)]
        public async Task EditorYConsultante_Reciben403EnLosEndpointsDeUsuarios(string rol)
        {
            var usuario = await CrearUsuarioAsync();
            var cliente = rol == Roles.Editor ? await Auth.CreateEditorClientAsync() : await Auth.CreateConsultanteClientAsync();
            var desde = UltimoId();

            var respuestas = new[]
            {
                await cliente.GetAsync("/api/usuarios"),
                await cliente.PostAsJsonAsync("/api/usuarios", new CreateUsuarioDto
                    { Email = "x@test.com", UserName = "x", Password = Password, Rol = Roles.Editor }),
                await cliente.PutAsJsonAsync($"/api/usuarios/{usuario.Id}", new UpdateUsuarioDto { Nombre = "X" }),
                await cliente.PutAsJsonAsync($"/api/usuarios/{usuario.Id}/rol", new CambiarRolDto { Rol = Roles.Administrador }),
                await cliente.PatchAsync($"/api/usuarios/{usuario.Id}/desactivar", null),
                await cliente.PostAsJsonAsync($"/api/usuarios/{usuario.Id}/reset-password", new ResetPasswordDto { PasswordNueva = "Otra12345" })
            };

            respuestas.Should().OnlyContain(r => r.StatusCode == HttpStatusCode.Forbidden);
            Historial(h => h.Id > desde).Should().BeEmpty();
        }
    }
}
