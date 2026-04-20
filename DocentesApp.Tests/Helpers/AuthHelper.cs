using DocentesApp.Application.DTOs.Auth;
using DocentesApp.Tests.Integration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

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

        public async Task<AuthResponseDto> RegisterUserAsync(
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

            var response = await _client.PostAsJsonAsync("/api/auth/register", dto);
            var body = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK, body);

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            authResponse.Should().NotBeNull();
            authResponse!.Token.Should().NotBeNullOrWhiteSpace();

            return authResponse;
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

        public async Task<string> GetUserTokenAsync()
        {
            var registerResponse = await RegisterUserAsync();
            return registerResponse.Token;
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
