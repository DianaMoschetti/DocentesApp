using DocentesApp.Data.Context;
using DocentesApp.Data.Identity;
using DocentesApp.Shared.Auth;
using AwesomeAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DocentesApp.Tests.Integration
{
    // Base existente con los roles viejos (Admin/User): al arrancar, SeedIdentityDataAsync los renombra
    // a Administrador/Consultante sin tocar los usuarios ni sus vínculos (no hace falta resembrar la base).
    public class RenombreDeRolesIntegrationTests : IClassFixture<RenombreDeRolesIntegrationTests.FactoryConRolesViejos>
    {
        private const string IdRolAdminViejo = "rol-admin-viejo";
        private const string IdRolUserViejo = "rol-user-viejo";
        private const string EmailConsultaViejo = "consulta-vieja@test.com";
        private const string PasswordConsultaViejo = "Viejo123";

        private readonly FactoryConRolesViejos _factory;
        private readonly Helpers.AuthHelper _auth;

        public RenombreDeRolesIntegrationTests(FactoryConRolesViejos factory)
        {
            _factory = factory;
            _auth = new Helpers.AuthHelper(factory);
        }

        [Fact]
        public async Task RolesViejos_SeRenombran_ConservandoIdsYUsuarios()
        {
            _factory.CreateClient(); // arranca el host (y los seeders)

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var roles = await db.Roles.AsNoTracking().ToListAsync();

            roles.Select(r => r.Name).Should().BeEquivalentTo(Roles.Todos);
            roles.Single(r => r.Name == Roles.Administrador).Id.Should().Be(IdRolAdminViejo);
            roles.Single(r => r.Name == Roles.Administrador).NormalizedName.Should().Be("ADMINISTRADOR");
            roles.Single(r => r.Name == Roles.Consultante).Id.Should().Be(IdRolUserViejo);
        }

        [Fact]
        public async Task UsuariosConRolesViejos_SeLogueanConLosNombresNuevos()
        {
            var admin = await _auth.LoginAsync("admin@docentesapp.com", "Admin123!");
            admin.Roles.Should().ContainSingle().Which.Should().Be(Roles.Administrador);

            var consulta = await _auth.LoginAsync(EmailConsultaViejo, PasswordConsultaViejo);
            consulta.Roles.Should().ContainSingle().Which.Should().Be(Roles.Consultante);
        }

        public class FactoryConRolesViejos : CustomWebApplicationFactory
        {
            protected override void SeedData(DocentesDbContext db)
            {
                base.SeedData(db);

                db.Roles.AddRange(
                    new IdentityRole { Id = IdRolAdminViejo, Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole { Id = IdRolUserViejo, Name = "User", NormalizedName = "USER" });

                // El admin ya existe (como en una base real): el seeder no lo recrea, solo renombra su rol
                var admin = UsuarioViejo("admin@docentesapp.com", "admin", "Admin123!");
                var consulta = UsuarioViejo(EmailConsultaViejo, "consultavieja", PasswordConsultaViejo);
                db.Users.AddRange(admin, consulta);

                db.UserRoles.AddRange(
                    new IdentityUserRole<string> { UserId = admin.Id, RoleId = IdRolAdminViejo },
                    new IdentityUserRole<string> { UserId = consulta.Id, RoleId = IdRolUserViejo });

                db.SaveChanges();
            }

            private static ApplicationUser UsuarioViejo(string email, string userName, string password)
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    UserName = userName,
                    NormalizedUserName = userName.ToUpperInvariant(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, password);
                return user;
            }
        }
    }
}
