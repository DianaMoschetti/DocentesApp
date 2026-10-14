using DocentesApp.Shared.DTOs.Auth;
using DocentesApp.Tests.Integration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
// [Diana desde v4.0 OBSOLETO] using FluentAssertions;
using AwesomeAssertions;
using DocentesApp.Shared.DTOs.Auth;

namespace DocentesApp.Tests.Helpers
{
    public class AuthHelper
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AuthHelper(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        // [Diana desde v4.0 OBSOLETO] el registro era público y devolvía el token del usuario nuevo;
        // ahora lo hace el Admin y devuelve los datos del usuario creado (RegisteredUserDto)
        //public async Task<AuthResponseDto> RegisterUserAsync(
        //    string? email = null,
        //    string? userName = null,
        //    string password = "Admin123")
        //{
        //    var unique = Guid.NewGuid().ToString("N")[..8];
        //
        //    var dto = new RegisterDto
        //    {
        //        Email = email ?? $"user-{unique}@test.com",
        //        UserName = userName ?? $"user{unique}",
        //        Password = password,
        //        Nombre = "Test",
        //        Apellido = "User"
        //    };
        //
        //    var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
        //    var body = await response.Content.ReadAsStringAsync();
        //
        //    response.StatusCode.Should().Be(HttpStatusCode.OK, body);
        //
        //    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        //    authResponse.Should().NotBeNull();
        //    authResponse!.Token.Should().NotBeNullOrWhiteSpace();
        //
        //    return authResponse;
        //}

        // Registra un usuario nuevo (rol User) con el cliente admin
        public async Task<RegisteredUserDto> RegisterUserAsync(
            string? email = null,
            string? userName = null,
            string password = "Admin123")
        {
            var unique = Guid.NewGuid().ToString("N")[..8];

            var dto = new RegisterDto
            {
                Email = email ?? $"user-{unique}@test.com",
                UserName = userName ?? $"user{unique}",
                Password = password,
                Nombre = "Test",
                Apellido = "User"
            };

            var admin = await CreateAdminClientAsync();
            var response = await admin.PostAsJsonAsync("/api/auth/register", dto);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);

            var registered = await response.Content.ReadFromJsonAsync<RegisteredUserDto>();
            registered.Should().NotBeNull();

            return registered!;
        }

        public async Task<AuthResponseDto> LoginAsync(string emailOrUserName, string password)
        {
            var dto = new LoginDto
            {
                EmailOrUserName = emailOrUserName,
                Password = password
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", dto);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeNullOrWhiteSpace();

            return authResponse;
        }

        // [Diana desde v4.0 OBSOLETO] el registro ya no devuelve token: se loguea el usuario User del seed de Identity
        //public async Task<string> GetUserTokenAsync()
        //{
        //    var registerResponse = await RegisterUserAsync();
        //    return registerResponse.Token;
        //}
        public async Task<string> GetUserTokenAsync()
        {
            var loginResponse = await LoginAsync("user@docentesapp.com", "User123!");
            return loginResponse.Token;
        }

        public async Task<string> GetAdminTokenAsync()
        {
            var loginResponse = await LoginAsync("admin@docentesapp.com", "Admin123!");
            return loginResponse.Token;
        }

        public HttpClient CreateAuthenticatedClient(string token)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<HttpClient> CreateAdminClientAsync()
        {
            var token = await GetAdminTokenAsync();
            return CreateAuthenticatedClient(token);
        }

        public async Task<HttpClient> CreateUserClientAsync()
        {
            var token = await GetUserTokenAsync();
            return CreateAuthenticatedClient(token);
        }
    }
}
