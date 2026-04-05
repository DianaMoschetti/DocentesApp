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
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, body);

        //response.StatusCode.Should().Be(HttpStatusCode.Created);

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
       //response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("Ya existe un docente con ese legajo.");
    }

   
    [Fact]
    public async Task DeleteDocente_WhenExistsAndHasNoDesignaciones_ReturnsNoContent()
    {
        // Arrange
        var unique = Guid.NewGuid().ToString("N")[..8];

        var dto = new CreateDocenteDto
        {
            Nombre = "Borrar",
            Apellido = "Test",
            Dni = $"55{unique[..6]}", // aseguro un dni unico para que no falle por duplicado
            Legajo = Random.Shared.Next(80000, 99999),
            Email = $"borrar-{unique}@test.com",
            MaxNivelAcademico = "Universitario",
            Observaciones = "Creado para test de delete"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/docentes", dto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var docente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        docente.Should().NotBeNull();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/docentes/{docente!.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/docentes/{docente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // diana: arreglar estos test cuando tenga bien definido que devuelve el get de docente, hay que mejorar ese dto para que traiga mas detalles
    [Fact]
    public async Task PatchContacto_WhenValid_ReturnsNoContent_AndUpdatesContacto()
    {
        // Arrange
        var random = Random.Shared.Next(60000000, 69999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(80000, 89999);
        var email = $"contacto-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Contacto",
            Apellido = "Test",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = "Universitario",
            Observaciones = "Docente para patch contacto"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        var patchDto = new UpdateContactoDocenteDto
        {
            Email = $"nuevo-{Guid.NewGuid():N}@test.com",
            EmailAlternativo = $"alt-{Guid.NewGuid():N}@test.com",
            Celular = "3415551234",
            Direccion = "Calle Patch 123"
        };

        // Act
        var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/contacto", patchDto);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

        var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updatedDocente.Should().NotBeNull();
        updatedDocente!.Email.Should().Be(patchDto.Email);
        updatedDocente.EmailAlternativo.Should().Be(patchDto.EmailAlternativo);
        updatedDocente.Celular.Should().Be(patchDto.Celular);
        updatedDocente.Direccion.Should().Be(patchDto.Direccion);
    }

    [Fact]
    public async Task PatchAcademico_WhenValid_ReturnsNoContent_AndUpdatesAcademico()
    {
        // Arrange
        var random = Random.Shared.Next(70000000, 79999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(90000, 94999);
        var email = $"academico-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Academico",
            Apellido = "Test",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = "Secundario",
            Observaciones = "Docente para patch academico"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        var patchDto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "Universitario"
        };

        // Act
        var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/academico", patchDto);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

        var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updatedDocente.Should().NotBeNull();
        updatedDocente!.MaxNivelAcademico.Should().Be("Universitario");
    }

    [Fact]
    public async Task PatchObservaciones_WhenValid_ReturnsNoContent_AndUpdatesObservaciones()
    {
        // Arrange
        var random = Random.Shared.Next(80000000, 89999999);
        var dni = random.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("es-AR"));
        var legajo = Random.Shared.Next(95000, 99999);
        var email = $"observaciones-{Guid.NewGuid():N}@test.com";

        var createDto = new CreateDocenteDto
        {
            Nombre = "Observaciones",
            Apellido = "Test",
            Dni = dni,
            Legajo = legajo,
            Email = email,
            MaxNivelAcademico = "Universitario",
            Observaciones = "Observación inicial"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/docentes", createDto);
        var createBody = await createResponse.Content.ReadAsStringAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, createBody);

        var createdDocente = await createResponse.Content.ReadFromJsonAsync<DocenteDto>();
        createdDocente.Should().NotBeNull();

        var patchDto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "Observación actualizada desde integration test"
        };

        // Act
        var patchResponse = await _client.PatchAsJsonAsync($"/api/docentes/{createdDocente!.Id}/observaciones", patchDto);
        var patchBody = await patchResponse.Content.ReadAsStringAsync();

        // Assert
        patchResponse.StatusCode.Should().Be(HttpStatusCode.NoContent, patchBody);

        var getResponse = await _client.GetAsync($"/api/docentes/{createdDocente.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedDocente = await getResponse.Content.ReadFromJsonAsync<DocenteDto>();
        updatedDocente.Should().NotBeNull();
        updatedDocente!.Observaciones.Should().Be("Observación actualizada desde integration test");
    }

    [Fact]
    public async Task PatchContacto_WhenDocenteDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var patchDto = new UpdateContactoDocenteDto
        {
            Email = "nuevo@test.com"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/docentes/999/contacto", patchDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        error.Should().NotBeNull();
        error!.Message.Should().Be("No se encontró el docente con ID 999.");
    }

    [Fact]
    public async Task PatchAcademico_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateAcademicoDocenteDto
        {
            MaxNivelAcademico = "   "
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/docentes/1/academico", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }
    [Fact]
    public async Task PatchObservaciones_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateObservacionesDocenteDto
        {
            Observaciones = "   "
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/docentes/1/observaciones", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }

    [Fact]
    public async Task PatchContacto_WhenBodyIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var patchDto = new UpdateContactoDocenteDto
        {
            Email = "   "
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/docentes/1/contacto", patchDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, body);
    }
}
