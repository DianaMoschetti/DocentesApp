using DocentesApp.API.Services;
using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.Auth;
using DocentesApp.Shared.DTOs.Usuarios;
using DocentesApp.Tests.Integration;
using AwesomeAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Services
{
    // Regla "siempre queda al menos un Administrador activo".
    // Por la API no se llega (nadie se desactiva ni se cambia el rol a sí mismo, así que el admin que opera queda):
    // se prueba llamando al servicio con otro actorId (como pasaría con requests simultáneos).
    // Usa un host propio para que el admin del seed sea el único Administrador.
    public class UsuarioServiceUltimoAdministradorTests : IDisposable
    {
        private const string ActorQueNoEsElAdmin = "otro-actor";
        private readonly CustomWebApplicationFactory _factory = new();

        public UsuarioServiceUltimoAdministradorTests()
        {
            _factory.CreateClient(); // arranca el host y corre el seed
        }

        public void Dispose() => _factory.Dispose();

        [Fact]
        public async Task DesactivarAlUnicoAdministrador_Devuelve409()
        {
            using var scope = _factory.Services.CreateScope();
            var (service, adminId) = await ServicioYAdminAsync(scope);

            var act = () => service.DesactivarAsync(adminId, ActorQueNoEsElAdmin);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Tiene que quedar al menos un Administrador activo.");
        }

        [Fact]
        public async Task QuitarleElRolAlUnicoAdministrador_Devuelve409()
        {
            using var scope = _factory.Services.CreateScope();
            var (service, adminId) = await ServicioYAdminAsync(scope);

            var act = () => service.CambiarRolAsync(adminId, Roles.Editor, ActorQueNoEsElAdmin);

            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Tiene que quedar al menos un Administrador activo.");
        }

        [Fact]
        public async Task ConOtroAdministradorActivo_SePuedeDesactivar()
        {
            using var scope = _factory.Services.CreateScope();
            var (service, adminId) = await ServicioYAdminAsync(scope);
            var otro = await service.CreateAsync(new CreateUsuarioDto
            {
                Email = "admin2@test.com", UserName = "admin2", Password = "Admin2345", Rol = Roles.Administrador
            });

            await service.DesactivarAsync(adminId, otro.Id);

            (await service.GetByIdAsync(adminId)).Activo.Should().BeFalse();
        }

        [Fact]
        public async Task UnAdministradorInactivo_NoCuentaComoElQueQueda()
        {
            using var scope = _factory.Services.CreateScope();
            var (service, adminId) = await ServicioYAdminAsync(scope);
            var otro = await service.CreateAsync(new CreateUsuarioDto
            {
                Email = "admin3@test.com", UserName = "admin3", Password = "Admin3456", Rol = Roles.Administrador
            });
            await service.DesactivarAsync(otro.Id, adminId); // el otro admin queda inactivo

            var act = () => service.DesactivarAsync(adminId, ActorQueNoEsElAdmin);

            await act.Should().ThrowAsync<ConflictException>();
        }

        private static async Task<(IUsuarioService Service, string AdminId)> ServicioYAdminAsync(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var admin = await userManager.FindByEmailAsync("admin@docentesapp.com");
            return (scope.ServiceProvider.GetRequiredService<IUsuarioService>(), admin!.Id);
        }
    }
}
