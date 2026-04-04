using System.Net;
using System.Net.Http.Json;
using DocentesApp.Application.Common.Responses;
using DocentesApp.Application.DTOs.Docentes;
using FluentAssertions;

namespace DocentesApp.Tests.Integration;

public class DocentesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DocentesControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDocentes_ReturnsOkAndList()
    {
        // Act
        var response = await _client.GetAsync("/api/docentes");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var docentes = await response.Content.ReadFromJsonAsync<List<ListDocenteDto>>();
        docentes.Should().NotBeNull();
        docentes.Should().HaveCountGreaterThanOrEqualTo(2);
        docentes.Should().Contain(d => d.NombreCompleto == "Perez, Juan");
    }

    [Fact]
    public async Task GetDocente_WhenExists_ReturnsOkAndDocente()
    {
        // Act
        var response = await _client.GetAsync("/api/docentes/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var docente = await response.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();
        docente!.Id.Should().Be(1);
        docente.NombreCompleto.Should().Be("Perez, Juan");
    }

    [Fact]
    public async Task GetDocente_WhenDoesNotExist_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/docentes/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task PostDocente_WhenValid_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Dni = "33.333.333",
            Legajo = 99999,
            Email = "carlos@test.com",
            MaxNivelAcademico = "Universitario",
            Observaciones = "Creado desde integration test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/docentes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var docente = await response.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();
        docente!.NombreCompleto.Should().Be("Lopez, Carlos");
        docente.Legajo.Should().Be(99999);

        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task PostDocente_WhenLegajoExists_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Nombre = "Duplicado",
            Apellido = "Legajo",
            Dni = "44.444.444",
            Legajo = 12345
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/docentes", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("Ya existe un docente con ese legajo.");
    }

    [Fact]
    public async Task DeleteDocente_WhenExistsAndHasNoDesignaciones_ReturnsNoContent()
    {
        // Arrange
        var dto = new CreateDocenteDto
        {
            Nombre = "Borrar",
            Apellido = "Test",
            Dni = "55.555.555",
            Legajo = 88888,
            Email = "borrar@test.com",
            MaxNivelAcademico = "Universitario",
            Observaciones = "Creado para test de delete"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/docentes", dto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var docente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/docentes/{docente!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/docentes/{docente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}