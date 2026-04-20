using DocentesApp.Application.DTOs.Auth;
using DocentesApp.Application.DTOs.Docentes;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DocentesApp.Tests.Integration
{
    public class AuthControllerIntegrationTests : IntegrationTestBase
    {
        public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Register_WhenValid_ReturnsOkAndToken()
        {
            // Act
            var result = await Auth.RegisterUserAsync();

            // Assert
            result.Email.Should().NotBeNullOrWhiteSpace();
            result.UserName.Should().NotBeNullOrWhiteSpace();
            result.Token.Should().NotBeNullOrWhiteSpace();
            result.Roles.Should().Contain("User");
        }

        [Fact]
        public async Task Login_WhenCredentialsAreValid_ReturnsOkAndToken()
        {
            // Arrange
            var unique = Guid.NewGuid().ToString("N")[..8];
            var email = $"login-{unique}@test.com";
            var userName = $"login{unique}";
            const string password = "Admin123";

            await Auth.RegisterUserAsync(email, userName, password);

            // Act
            var result = await Auth.LoginAsync(email, password);

            // Assert
            result.Email.Should().Be(email);
            result.UserName.Should().Be(userName);
            result.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_WhenCredentialsAreInvalid_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto
            {
                EmailOrUserName = "noexiste@test.com",
                Password = "WrongPassword"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/auth/login", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new CreateDocenteDto
            {
                Nombre = "Sin",
                Apellido = "Token",
                Dni = "33.333.333",
                Legajo = 123456,
                Email = "sintoken@test.com",
                MaxNivelAcademico = "Universitario",
                Observaciones = "Test sin token"
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/docentes", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithUserRole_ReturnsForbidden()
        {
            // Arrange
            var userClient = await Auth.CreateUserClientAsync();

            var dto = new CreateDocenteDto
            {
                Nombre = "Con",
                Apellido = "UserRole",
                Dni = "44.444.444",
                Legajo = 123457,
                Email = "userrole@test.com",
                MaxNivelAcademico = "Universitario",
                Observaciones = "Test con rol User"
            };

            // Act
            var response = await userClient.PostAsJsonAsync("/api/docentes", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithAdminRole_ReturnsCreated()
        {
            // Arrange
            var adminClient = await Auth.CreateAdminClientAsync();
            var unique = Guid.NewGuid().ToString("N")[..8];

            var dto = new CreateDocenteDto
            {
                Nombre = "Admin",
                Apellido = "Ok",
                Dni = "55.555.555",
                Legajo = Random.Shared.Next(200000, 299999),
                Email = $"admin-{unique}@test.com",
                MaxNivelAcademico = "Universitario",
                Observaciones = "Creado por admin"
            };

            // Act
            var response = await adminClient.PostAsJsonAsync("/api/docentes", dto);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created, body);

            var docente = await response.Content.ReadFromJsonAsync<DocenteDto>();
            docente.Should().NotBeNull();
            docente!.NombreCompleto.Should().Be("Ok, Admin");
        }
    }
}